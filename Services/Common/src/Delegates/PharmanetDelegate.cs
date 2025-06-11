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
        // <param name="hl7base64Message">The base64 encoded HL7v2 Message</param>
        // <returns>The resulting corrected base64 encoded message</returns>
        private string TrimBadCharactersInMessage(string hl7base64Message = @"")
        {
            byte[] bytes = Convert.FromBase64String(hl7base64Message);
            byte[] newBytes = new byte[bytes.Length];

            // This log statement logs sensitive health information - use it only for debugging in a development environment
            // Logger.LogDebug(this.logger, $"RESPONSE B64='{hl7base64Message}'");

            Span<byte> span = bytes;
            int i = 0;
            foreach (byte aByte in span)
            {
                newBytes[i] = 0x00;
                if ((aByte > 0x00) && (aByte <= 0xff))  // only bytes in UTF8 range
                {
                    newBytes[i] = aByte;
                    i++;
                }
                else
                {
                    Logger.LogDebug(this.logger, $"WORKAROUND: Removed {aByte:X4} character from HL7v2 response.");
                }
            }
            string b64ResultStr = Convert.ToBase64String(newBytes, 0, i);

            // This log statement logs sensitive health information - use it only for debugging in a development environment
            // Logger.LogDebug(this.logger, $"UPDATED RESPONSE B64='{b64ResultStr}'");

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
        /// <param name="traceId">The value used to track messages from API Gateway.</param>
        /// <param name="isHealthCheck">A boolean indicating if this request is a health check or a transaction.</param>
        /// <returns>A PharmanetMessage response.</returns>
        public async Task<RequestResult<PharmanetMessageModel>> SubmitRequest(PharmanetMessageModel request, string traceId, bool isHealthCheck)
        {
            // Logger.LogDebug(this.logger, $"Transaction UUID: {request.TransactionId}: PharmanetDelegate.SubmitRequest start");

            RequestResult<PharmanetMessageModel> requestResult = new RequestResult<PharmanetMessageModel>();

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            string jsonOutput = JsonSerializer.Serialize<PharmanetMessageModel>(request, options);

            using HttpContent content = new StringContent(jsonOutput);

            try
            {
                Uri delegateUri = new Uri(isHealthCheck ? this.pharmanetDelegateConfig.HealthCheckEndpoint : this.pharmanetDelegateConfig.Endpoint);

                Logger.LogInformation(this.logger, $"Trace ID: {traceId}: Transaction UUID: {request.TransactionId}: PharmanetDelegate.SubmitRequest: Sending message to PharmaNet: {delegateUri}");
                // This log statement logs sensitive health information - use it only for debugging in a development environment
                // Logger.LogDebug(this.logger, $"PharmanetDelegate Proxy POST {delegateUri}. Payload: {jsonOutput}");

                HttpResponseMessage response = await this.httpClient.PostAsync(delegateUri, content).ConfigureAwait(true);

                Logger.LogInformation(this.logger, $"Transaction UUID: {request.TransactionId}: PharmanetDelegate.SubmitRequest: Received response from PharmaNet with Status Code: {response.StatusCode}.");

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
                    // Logger.LogDebug(this.logger, $"Transaction UUID: {request.TransactionId}: PharmanetDelegate.SubmitRequest: Response success, extracting response content...");
                    string? result = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
                    // Logger.LogDebug(this.logger, $"Transaction UUID: {request.TransactionId}: PharmanetDelegate.SubmitRequest: Extracting response content. Deserializing result: {result}");
                    PharmanetMessageModel? responseMessage = JsonSerializer.Deserialize<PharmanetMessageModel>(result);
                    // Logger.LogDebug(this.logger, $"Transaction UUID: {request.TransactionId}: PharmanetDelegate.SubmitRequest: Deserialized message. Building response...");

                    responseMessage!.Hl7Message = TrimBadCharactersInMessage(responseMessage!.Hl7Message); // Workaround stray chars from Delegate
                    requestResult.Payload = responseMessage;
                    // This log statement does not log sensitive health information, even though it looks like it might
                    Logger.LogDebug(this.logger, $"Transaction UUID: {request.TransactionId}: PharmanetDelegate Proxy Response: {responseMessage}");
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

            // Logger.LogDebug(this.logger, $"Transaction UUID: {request.TransactionId}: PharmanetDelegate.SubmitRequest end ");
            return requestResult;
        }
    }
}
