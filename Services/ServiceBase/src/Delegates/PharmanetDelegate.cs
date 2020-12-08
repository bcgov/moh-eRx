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
namespace Health.PharmaNet.Delegates
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net.Mime;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Health.PharmaNet.Models;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The Pharmanet Delegate, which communicates directly to the Pharmanet proxy service.
    /// </summary>
    public class PharmanetDelegate : IPharmanetDelegate
    {
        private const string ConfigurationSectionKey = "PharmanetProxy";

        private static readonly HttpClientHandler ClientHandler = new HttpClientHandler();

        private static readonly HttpClient Client = new HttpClient(ClientHandler);

        private readonly IConfiguration configuration;

        private readonly ILogger logger;

        private readonly PharmanetDelegateConfig pharmanetDelegateConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="PharmanetDelegate"/> class.
        /// </summary>
        /// <param name="logger">Injected Logger Provider.</param>
        /// <param name="configuration">The injected configuration provider.</param>
        public PharmanetDelegate(ILogger<PharmanetDelegate> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.pharmanetDelegateConfig = new PharmanetDelegateConfig();
            this.configuration.Bind(ConfigurationSectionKey, this.pharmanetDelegateConfig);

            // Load Certificate
            string certPath = this.pharmanetDelegateConfig.ClientCertificatePath;
            string certPassword = this.pharmanetDelegateConfig.ClientCertificatePassword;

            ClientHandler.ClientCertificates.Add(new X509Certificate2(System.IO.File.ReadAllBytes(certPath), certPassword));
        }

        /// <summary>
        /// Submit a PharmanetMessage to Pharmanet System.
        /// </summary>
        /// <param name="request">The PharmanetMessage request containing HL7v2 base 64 payload.</param>
        /// <returns>A PharmanetMessage response.</returns>
        public async Task<RequestResult<PharmanetDelegateMessageModel>> SubmitRequest(PharmanetDelegateMessageModel request)
        {
            RequestResult<PharmanetDelegateMessageModel> requestResult = new RequestResult<PharmanetDelegateMessageModel>();

            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

            byte[] authdata = Encoding.ASCII.GetBytes(this.pharmanetDelegateConfig.Username + ":" + this.pharmanetDelegateConfig.Password);

            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", System.Convert.ToBase64String(authdata));

            string jsonOutput = JsonSerializer.Serialize<PharmanetDelegateMessageModel>(request);

            using (HttpContent content = new StringContent(jsonOutput))
            {
                Uri delegateUri = new Uri(this.pharmanetDelegateConfig.Endpoint);

                HttpResponseMessage response = await Client.PostAsync(delegateUri, content).ConfigureAwait(true);
                requestResult.IsSuccessStatusCode = response.IsSuccessStatusCode;

                if (!requestResult.IsSuccessStatusCode)
                {
                    this.logger.LogError($"PharmanetDelegate Proxy call returned with StatusCode := {response.StatusCode}.");
                    return requestResult;
                }
                else
                {
                    try
                    {
                        string? result = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
                        PharmanetDelegateMessageModel? responseMessage = JsonSerializer.Deserialize<PharmanetDelegateMessageModel>(result);
                        requestResult.Payload = responseMessage;
                    }
#pragma warning disable CA1031 // Do not catch general exception types
                    catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
                    {
                        this.logger.LogError($"PharmanetDelegate Exception := {ex.Message}.");

                        requestResult.IsSuccessStatusCode = false;
                        requestResult.ResultErrorMessage = ex.Message;
                        return requestResult;
                    }
                }

                return requestResult;
            }
        }
    }
}
