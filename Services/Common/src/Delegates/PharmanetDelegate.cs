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
    using System.Text.Encodings.Web;
    using System.Text.Json;
    using System.Text.Unicode;
    using System.Threading.Tasks;

    using Health.PharmaNet.Models;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The Pharmanet Delegate, which communicates directly to the Pharmanet proxy service.
    /// </summary>
    public class PharmanetDelegate : IPharmanetDelegate
    {
        private readonly IConfiguration configuration;
        private readonly ILogger logger;
        private readonly HttpClient httpClient;
        private readonly PharmanetDelegateConfig pharmanetDelegateConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="PharmanetDelegate"/> class.
        /// </summary>
        /// <param name="client">The injected HttpClient from service injection.</param>
        /// <param name="logger">Injected Logger Provider.</param>
        /// <param name="configuration">The injected configuration provider.</param>
        public PharmanetDelegate(HttpClient client, ILogger<PharmanetDelegate> logger, IConfiguration configuration)
        {
            this.httpClient = client;
            this.logger = logger;
            this.configuration = configuration;
            this.pharmanetDelegateConfig = new PharmanetDelegateConfig();
            this.configuration.Bind(PharmanetDelegateConfig.ConfigurationSectionKey, this.pharmanetDelegateConfig);
        }

        /// <summary>
        /// Submit a PharmanetMessage to Pharmanet System.
        /// </summary>
        /// <param name="request">The PharmanetMessage request containing HL7v2 base 64 payload.</param>
        /// <returns>A PharmanetMessage response.</returns>
        public async Task<RequestResult<PharmanetDelegateMessageModel>> SubmitRequest(PharmanetDelegateMessageModel request)
        {
            RequestResult<PharmanetDelegateMessageModel> requestResult = new RequestResult<PharmanetDelegateMessageModel>();

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            string jsonOutput = JsonSerializer.Serialize<PharmanetDelegateMessageModel>(request, options);

            using HttpContent content = new StringContent(jsonOutput);

            try
            {
                Uri delegateUri = new Uri(this.pharmanetDelegateConfig.Endpoint);

                this.logger.LogDebug($"PharmanetDelegate Proxy POST {delegateUri}. Payload: {jsonOutput}");

                HttpResponseMessage response = await this.httpClient.PostAsync(delegateUri, content).ConfigureAwait(true);
                requestResult.IsSuccessStatusCode = response.IsSuccessStatusCode;
                requestResult.StatusCode = response.StatusCode;

                if (!requestResult.IsSuccessStatusCode)
                {
                    string msg = "PharmanetDelegate Proxy call returned with StatusCode := " + response.StatusCode;
                    this.logger.LogError(msg);
                    requestResult.ErrorMessage = msg;
                    return requestResult;
                }
                else
                {
                    string? result = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
                    PharmanetDelegateMessageModel? responseMessage = JsonSerializer.Deserialize<PharmanetDelegateMessageModel>(result);
                    requestResult.Payload = responseMessage;
                    this.logger.LogDebug($"PharmanetDelegate Proxy Response: {responseMessage}");
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                this.logger.LogError($"PharmanetDelegate Exception := {ex.Message}.");

                requestResult.IsSuccessStatusCode = false;
                requestResult.ErrorMessage = ex.Message;
                requestResult.StatusCode = System.Net.HttpStatusCode.InternalServerError;

                return requestResult;
            }

            return requestResult;
        }
    }
}
