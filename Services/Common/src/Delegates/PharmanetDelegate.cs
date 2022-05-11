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
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Health.PharmaNet.Common.Logging;
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


        // <summary>
        //  Trims Bad (out of band) characters from the UTF-8 HL7v2 Message.
        // </summary>
        // <param name="b64Message">The base64 encoded HL7v2 Message</param>
        // <returns>The resulting corrected base64 encoded message</returns>
        private PharmanetMessageModel TrimBadCharactersInMessage(PharmanetMessageModel? message)
        {
            Logger.LogDebug(this.logger, "Checking if need to trim extraneous characters from end of HL7v2 Response...");
            byte[] bytes = Convert.FromBase64String(message!.Hl7Message);

            for (int i=(bytes.Length-1); i>=0; i--)
            {
                Logger.LogDebug(this.logger, $"Checking bytes[{i}] = {bytes[i]}");
                if (bytes[i] == 0x0d || bytes[i] == 0x0a)  // trim back to last 'nl' or 'cr' character
                {
                    Logger.LogDebug(this.logger, $"Trimming completed at {i}");
                    break;
                }
                Logger.LogDebug(this.logger, $"Trimmed out of band byte from HL7v2: '{bytes[i]}'");
                bytes[i] = 0x00; // null out the character to terminate string length.
            }
            message!.Hl7Message = Convert.ToBase64String(bytes);
            Logger.LogInformation(this.logger, "Trimmed any extraneous characters from end of HL7v2 Response.");
            return message;
        }

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
        public async Task<RequestResult<PharmanetMessageModel>> SubmitRequest(PharmanetMessageModel request)
        {
            RequestResult<PharmanetMessageModel> requestResult = new RequestResult<PharmanetMessageModel>();

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            string jsonOutput = JsonSerializer.Serialize<PharmanetMessageModel>(request, options);

            using HttpContent content = new StringContent(jsonOutput);

            try
            {
                Uri delegateUri = new Uri(this.pharmanetDelegateConfig.Endpoint);

                Logger.LogDebug(this.logger, $"PharmanetDelegate Proxy POST {delegateUri}. Payload: {jsonOutput}");

                HttpResponseMessage response = await this.httpClient.PostAsync(delegateUri, content).ConfigureAwait(true);
                requestResult.IsSuccessStatusCode = response.IsSuccessStatusCode;
                requestResult.StatusCode = response.StatusCode;

                if (!requestResult.IsSuccessStatusCode)
                {
                    string msg = "PharmanetDelegate Proxy call returned with StatusCode := " + response.StatusCode;
                    Logger.LogDebug(this.logger, msg);
                    requestResult.ErrorMessage = msg;
                    return requestResult;
                }
                else
                {
                    string? result = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
                    PharmanetMessageModel? responseMessage = JsonSerializer.Deserialize<PharmanetMessageModel>(result);
                    
                    responseMessage = TrimBadCharactersInMessage(responseMessage!); // Workaround stray chars from Delegate
                    requestResult.Payload = responseMessage;
                    Logger.LogDebug(this.logger, $"PharmanetDelegate Proxy Response: {responseMessage}");
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                Logger.LogException(this.logger, $"PharmanetDelegate Exception Occurred.", ex);

                requestResult.IsSuccessStatusCode = false;
                requestResult.ErrorMessage = ex.Message;
                requestResult.StatusCode = System.Net.HttpStatusCode.InternalServerError;

                return requestResult;
            }

            return requestResult;
        }
    }
}
