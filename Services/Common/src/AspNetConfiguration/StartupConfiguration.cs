//-------------------------------------------------------------------------
// Copyright © 2020 Province of British Columbia
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//-------------------------------------------------------------------------
namespace Health.PharmaNet.Common.AspNetConfiguration
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Reflection;
    using System.Text.Encodings.Web;
    using System.Text.Json;
    using System.Text.Unicode;
    using System.Threading.Tasks;

    using Health.PharmaNet.Common.Authorization;
    using Health.PharmaNet.Common.Authorization.Policy;
    using Health.PharmaNet.Common.Swagger;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.ResponseCompression;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Logging;
    using Microsoft.IdentityModel.Tokens;

#pragma warning disable CA1303 // Do not pass literals as localized parameters

    /// <summary>
    /// The startup configuration class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1506:Avoid excessive class coupling", Justification = "Team decision")]
    public class StartupConfiguration
    {
        private readonly IWebHostEnvironment environment;
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartupConfiguration"/> class.
        /// </summary>
        /// <param name="config">The configuration provider.</param>
        /// <param name="env">The environment variables provider.</param>
        public StartupConfiguration(IConfiguration config, IWebHostEnvironment env)
        {
            this.environment = env;
            this.configuration = config;
            this.Logger = this.GetStartupLogger();
        }

        /// <summary>
        /// Gets the Startup Logger.
        /// </summary>
        public ILogger Logger { get; private set; }

        /// <summary>
        /// Configures the http services.
        /// </summary>
        /// <param name="services">The service collection provider.</param>
        public void ConfigureHttpServices(IServiceCollection services)
        {
            this.Logger.LogDebug("Configure Http Services...");

            services.AddControllers();

            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                options.EnableForHttps = true;
            });

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHealthChecks();

            services.AddHttpClient("HttpClientWithSSLUntrusted").ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback =
                    (httpRequestMessage, cert, cetChain, policyErrors) =>
                    {
                        return true;
                    },
            });

            services
                .AddRazorPages()
                .SetCompatibilityVersion(CompatibilityVersion.Latest)
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.WriteIndented = true;
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                });
        }

        /// <summary>
        /// Configures the auth services for json web token bearer.
        /// </summary>
        /// <param name="services">The injected services provider.</param>
        public void ConfigureAuthServicesForJwtBearer(IServiceCollection services)
        {
            bool debugEnabled = this.environment.IsDevelopment() || this.configuration.GetValue<bool>("EnableDebug", true);
            this.Logger.LogDebug($"Debug configuration is {debugEnabled}");

            // Displays sensitive data from the jwt if the environment is development only
            IdentityModelEventSource.ShowPII = debugEnabled;

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = true;
                options.IncludeErrorDetails = true;

                this.configuration.GetSection(ConfigurationSections.OpenIdConnect).Bind(options);

                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidIssuer = options.Authority,
                    ValidateIssuerSigningKey = true,
                    RequireSignedTokens = true,
                };
                options.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = (ctx) => { return this.OnAuthenticationFailed(ctx); },
                };
            });

            services.AddAuthorization(options =>
                        {
                            string claimsIssuer = this.configuration.GetSection(ConfigurationSections.OpenIdConnect).GetValue<string>("ClaimsIssuer");
                            string scopes = this.configuration.GetSection(ConfigurationSections.OpenIdConnect).GetValue<string>("Scope");

                            string[] scope = scopes.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                            options.AddPolicy(FhirScopesPolicy.Access, policy =>
                            {
                                policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                                policy.RequireAuthenticatedUser();
                                policy.Requirements.Add(new HasScopesRequirement(scope, claimsIssuer));
                            });
                        });

            // register the  handlers
            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
        }

        /// <summary>
        /// Configures the swagger services.
        /// </summary>
        /// <param name="services">The service collection provider.</param>
        public void ConfigureSwaggerServices(IServiceCollection services)
        {
            services.Configure<SwaggerSettings>(this.configuration.GetSection(nameof(SwaggerSettings)));
            var xmlFile = $"{Assembly.GetCallingAssembly() !.GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            services
                .AddApiVersionWithExplorer()
                .AddSwaggerOptions()
                .AddSwaggerGen(options =>
                {
                    options.IncludeXmlComments(xmlPath);
                });
        }

        /// <summary>
        /// Configures Forward proxies.
        /// </summary>
        /// <param name="services">The service collection to add forward proxies into.</param>
        public void ConfigureForwardHeaders(IServiceCollection services)
        {
            IConfigurationSection section = this.configuration.GetSection("ForwardProxies");
            bool enabled = section.GetValue<bool>("Enabled");
            this.Logger.LogInformation($"Forward Proxies enabled: {enabled}");
            if (enabled)
            {
                this.Logger.LogDebug("Configuring Forward Headers");
                IPAddress[] proxyIPs = section.GetSection("KnownProxies").Get<IPAddress[]>() ?? Array.Empty<IPAddress>();
                services.Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders = ForwardedHeaders.All;
                    options.RequireHeaderSymmetry = false;
                    options.ForwardLimit = null;
                    options.KnownNetworks.Clear();
                    options.KnownProxies.Clear();
                    foreach (IPAddress ip in proxyIPs)
                    {
                        options.KnownProxies.Add(ip);
                    }
                });
            }
        }

        /// <summary>
        /// Configures the app to use auth.
        /// </summary>
        /// <param name="app">The application builder provider.</param>
        public void UseAuth(IApplicationBuilder app)
        {
            this.Logger.LogDebug("Use Auth...");

            // Enable jwt authentication
            app.UseAuthentication();
            app.UseAuthorization();
        }

        /// <summary>
        /// Configures the app to use x-forwarded-for headers to obtain the real client IP.
        /// </summary>
        /// <param name="app">The application builder provider.</param>
        public void UseForwardHeaders(IApplicationBuilder app)
        {
            IConfigurationSection section = this.configuration.GetSection("ForwardProxies");
            bool enabled = section.GetValue<bool>("Enabled");
            this.Logger.LogInformation($"Forward Proxies enabled: {enabled}");
            if (enabled)
            {
                this.Logger.LogDebug("Using Forward Headers");
                string basePath = section.GetValue<string>("BasePath");
                if (!string.IsNullOrEmpty(basePath))
                {
                    this.Logger.LogInformation($"Forward BasePath is set to {basePath}, setting PathBase for app");
                    app.UsePathBase(basePath);
                    app.Use(async (context, next) =>
                    {
                        context.Request.PathBase = basePath;
                        await next.Invoke().ConfigureAwait(true);
                    });
                    app.UsePathBase(basePath);
                }

                this.Logger.LogInformation("Enabling Use Forward Header");
                app.UseForwardedHeaders();
            }
        }

        /// <summary>
        /// Configures the app to use http.
        /// </summary>
        /// <param name="app">The application builder provider.</param>
        public void UseHttp(IApplicationBuilder app)
        {
            if (this.environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseRouting();

            // Enable health endpoint for readiness probe
            app.UseHealthChecks("/health");

            // Enable CORS
            string enableCors = this.configuration.GetValue<string>("AllowOrigins", string.Empty);
            if (!string.IsNullOrEmpty(enableCors))
            {
                app.UseCors(builder =>
                {
                    builder
                        .WithOrigins(enableCors)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            }

            app.UseResponseCompression();

            // Setup response secure headers
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("X-Xss-Protection", "1; mode=block");
                await next().ConfigureAwait(true);
            });
        }

        /// <summary>
        /// Configures the app to to use content security policies.
        /// </summary>
        /// <param name="app">The application builder provider.</param>
        public void UseContentSecurityPolicy(IApplicationBuilder app)
        {
            IConfigurationSection cspSection = this.configuration.GetSection("ContentSecurityPolicy");
            string connectSrc = cspSection.GetValue<string>("connect-src", string.Empty);
            string frameSrc = cspSection.GetValue<string>("frame-src", string.Empty);
            string scriptSrc = cspSection.GetValue<string>("script-src", string.Empty);
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Content-Security-Policy", $"default-src 'self'; script-src 'self' 'unsafe-eval' {scriptSrc}; connect-src 'self' {connectSrc}; img-src 'self' data:; style-src 'self' 'unsafe-inline';base-uri 'self';form-action 'self'; font-src 'self'; frame-src 'self' {frameSrc}");
                await next().ConfigureAwait(true);
            });
        }

        /// <summary>
        /// Configures the app to use swagger.
        /// </summary>
        /// <param name="app">The application builder provider.</param>
        public void UseSwagger(IApplicationBuilder app)
        {
            this.Logger.LogDebug("Use Swagger...");

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            app.UseSwaggerDocuments();
        }

        /// <summary>
        /// Configures the app to use Rest services.
        /// </summary>
        /// <param name="app">The application builder provider.</param>
        public void UseRest(IApplicationBuilder app)
        {
            this.Logger.LogDebug("Use Rest...");
            app.UseEndpoints(routes =>
            {
                routes.MapControllers();
            });
        }

        private ILogger GetStartupLogger()
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddConfiguration(this.configuration);
            });

            return loggerFactory.CreateLogger("Startup");
        }

        /// <summary>
        /// Handles authentication failures.
        /// </summary>
        /// <param name="context">The authentication failed context.</param>
        /// <returns>An async task.</returns>
        private Task OnAuthenticationFailed(AuthenticationFailedContext context)
        {
            this.Logger.LogDebug("OnAuthenticationFailed...");
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                State = "AuthenticationFailed",
                Message = context.Exception.ToString(),
            }));
        }
    }
}
