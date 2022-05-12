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
    using System.Text.RegularExpressions;
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
        private string TrimBadCharactersInMessage(string hl7base64Message = @"")
        {
            byte[] bytes = Convert.FromBase64String(hl7base64Message);
            string hl7v2 = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            int origLen = hl7v2.Length;
            string[] badChars = { "\x00BD", "\x00BF", "\x00EF"};

            Logger.LogDebug(this.logger, hl7v2);

            foreach (string badCharStr in badChars)
            {
                hl7v2 = hl7v2.Replace(badCharStr, String.Empty, StringComparison.OrdinalIgnoreCase);
            }
  
            if (hl7v2.Length != origLen) 
            {
                int trimmed = origLen - hl7v2.Length;
                Logger.LogInformation(this.logger, $"Trimmed {trimmed} extended characters from HL7v2 Response.");
            }
            else 
            {
                Logger.LogDebug(this.logger, "No characters trimmmed from HL7v2 Response.");
            }

            bytes = Encoding.UTF8.GetBytes(hl7v2, 0, hl7v2.Length);
            string b64ResultStr = Convert.ToBase64String(bytes);

            return b64ResultStr;
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

                    responseMessage!.Hl7Message = TrimBadCharactersInMessage(responseMessage!.Hl7Message); // Workaround stray chars from Delegate
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
