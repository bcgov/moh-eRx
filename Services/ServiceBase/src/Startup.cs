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
namespace Health.PharmaNet
{
    using System;

    using Health.PharmaNet.Authorization;
    using Health.PharmaNet.Authorization.Policy;
    using Health.PharmaNet.Common.AspNetConfiguration;

    using Health.PharmaNet.Delegates;
    using Health.PharmaNet.Parsers;
    using Health.PharmaNet.Services;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Configures the application during startup.
    /// </summary>
    public class Startup
    {
        private readonly StartupConfiguration startupConfig;
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="env">The injected Environment provider.</param>
        /// <param name="configuration">The injected configuration provider.</param>
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            this.startupConfig = new StartupConfiguration(configuration, env);
            this.configuration = configuration;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The injected services provider.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            this.startupConfig.ConfigureForwardHeaders(services);
            this.startupConfig.ConfigureHttpServices(services);
            this.startupConfig.ConfigureAuthServicesForJwtBearer(services);

            services.AddAuthorization(options =>
  {
      string claimsIssuer = this.configuration.GetSection(ConfigurationSections.OpenIdConnect).GetValue<string>("ClaimsIssuer");
      string scopes = this.configuration.GetSection(ConfigurationSections.OpenIdConnect).GetValue<string>("Scope");

      string[] scope = scopes.Split(" ", StringSplitOptions.RemoveEmptyEntries);

      options.AddPolicy(Hl7v2ScopesPolicy.MessageTypeScopeAccess, policy =>
      {
          policy.Requirements.Add(new Hl7v2AuthorizationRequirement(this.configuration));
      });
  });

            services.AddSingleton<IAuthorizationHandler, Hl7v2AuthorizationHandler>();

            this.startupConfig.ConfigureSwaggerServices(services);

            services.AddCors(options =>
            {
                options.AddPolicy("allowAny", policy =>
                {
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            // Add Services
            services.AddTransient<IPharmanetDelegate, PharmanetDelegate>();
            services.AddTransient<IPharmanetService, PharmanetService>();
            services.AddTransient<IHl7Parser, Hl7Parser>();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        public void Configure(IApplicationBuilder app)
        {
            this.startupConfig.UseForwardHeaders(app);
            this.startupConfig.UseSwagger(app);
            this.startupConfig.UseHttp(app);
            this.startupConfig.UseAuth(app);
            this.startupConfig.UseRest(app);
        }
    }
}
