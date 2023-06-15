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
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.Text.Json;
using System.Text.Json.Serialization;

using PharmaNet.Client.Services;
using PharmaNet.Client.Models;

namespace PharmaNet.Client
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();

            IConfiguration configuration = InitConfig();

            // Application code should start here.

            IServiceProvider serviceProvider = new ServiceCollection()
                .AddSingleton<IAuthService, AuthService>()
                .AddSingleton<IPharmanetService, PharmanetService>()
                .AddSingleton<IConfiguration>(configuration)
                .BuildServiceProvider();

            Console.WriteLine("Running Example Pharmanet API Client.");

            IAuthService authService = serviceProvider.GetService<IAuthService>();

            Boolean useKeyFile = configuration.GetValue<Boolean>("UseKeyFile");

            string token = string.Empty;

            // Choose to authenticate with key file or with ID and secret
            if (useKeyFile) {
                token = authService.AuthenticateUsingSignedJWT();
            }
            else {
                token = authService.AuthenticateUsingClientCredentials();
            }

            // Parse the access token
            token = JsonSerializer.Deserialize<AccessTokenResponse>(token).access_token;

            // Make the request to the API
            PostService postService = new PostService(configuration, token);

            Console.WriteLine(postService.PostRequest().Result);

            await host.RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args);

        private static IConfiguration InitConfig()
        {
            var cwd = Directory.GetCurrentDirectory();
            Console.WriteLine("cwd = {0}", cwd);
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                .AddJsonFile(cwd + $"/appsettings.json", true, true)
                .AddJsonFile(cwd + $"/appsettings.{env}.json", true, true)
                .AddJsonFile(cwd + $"/appsettings.local.json", true, true)
                .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}
