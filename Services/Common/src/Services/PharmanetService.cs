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
namespace Health.PharmaNet.Services
{
    using System;
    using System.Threading.Tasks;

    using Health.PharmaNet.Common.Logging;
    using Health.PharmaNet.Delegates;
    using Health.PharmaNet.Models;

    using Hl7.Fhir.Model;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The PharmanetService.
    /// </summary>
    public class PharmanetService : IPharmanetService
    {
        private readonly IPharmanetDelegate pharmanetDelegate;
        private readonly ILogger<PharmanetService> logger;
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="PharmanetService"/> class.
        /// </summary>
        /// <param name="logger">Injected Logger Provider.</param>
        /// <param name="pharmanetDelegate">Injected delegate.</param>
        /// <param name="configuration">Injected configuration.</param>
        public PharmanetService(
            ILogger<PharmanetService> logger,
            IPharmanetDelegate pharmanetDelegate,
            IConfiguration configuration)
        {
            this.pharmanetDelegate = pharmanetDelegate;
            this.logger = logger;
            this.configuration = configuration;
        }

        /// <summary>
        /// Submit Request to Pharmanet.
        /// </summary>
        /// <param name="request">The DocumentReference to be submitted.</param>
        /// <returns>Returns a DocumentReference containing the response from PharmaNet.</returns>
        public async Task<RequestResult<DocumentReference>> SubmitRequest(DocumentReference request)
        {
            Logger.LogInformation(this.logger, $"PharmanetService.SubmitRequest start");

            RequestResult<DocumentReference> response = new RequestResult<DocumentReference>();
            bool base64Encode = this.configuration.GetSection(PharmanetDelegateConfig.ConfigurationSectionKey).GetValue<bool>("Base64EncodeHl7Message");
            Logger.LogInformation(this.logger, $"PharmanetService.SubmitRequest: UUID exists in FHIR? {request.MasterIdentifier != null} ");
            PharmanetMessageModel requestMessage = PharmanetDelegateAdapter.ToPharmanetMessageModel(request, base64Encode);
            Logger.LogInformation(this.logger, $"Transaction UUID: {requestMessage.TransactionId}: PharmanetService.SubmitRequest: PharmanetMessageModel created.");

            try
            {
                // This log statement logs sensitive health information - use it only for debugging in a development environment
                // Logger.LogDebug(this.logger, $"Pharmanet Request: {requestMessage.Hl7Message}");

                RequestResult<PharmanetMessageModel> result = await this.pharmanetDelegate.SubmitRequest(requestMessage).ConfigureAwait(true);

                response.StatusCode = result.StatusCode;
                response.ErrorMessage = result.ErrorMessage;

                if (result.IsSuccessStatusCode)
                {
                    PharmanetMessageModel? message = result.Payload;

                    // This log statement logs sensitive health information - use it only for debugging in a development environment
                    // this.logger.LogDebug($"Pharmanet Response: {message!.Hl7Message}");

                    Logger.LogInformation(this.logger, $"Transaction UUID: {requestMessage.TransactionId}: PharmanetService.SubmitRequest: Building DocumentReference response...");
                    ResourceReference reference = PharmanetDelegateAdapter.RelatedToDocumentReference(request);
                    response.Payload = PharmanetDelegateAdapter.ToDocumentReference(message!, reference);
                    Logger.LogInformation(this.logger, $"Transaction UUID: {requestMessage.TransactionId}: PharmanetService.SubmitRequest: DocumentReference response built.");

                    // This log statement does not log sensitive health information, even though it looks like it might
                    this.logger.LogDebug($"FHIR Response: {response!.Payload.ToString()}");

                    response.IsSuccessStatusCode = true;
                }
                else
                {
                    Logger.LogDebug(this.logger, $"Pharmanet Response Error: {result.ErrorMessage}");
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                Logger.LogException(this.logger, "Pharmanet Exception.", ex);

                response.IsSuccessStatusCode = false;
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.ErrorMessage = ex.Message;
            }

            Logger.LogInformation(this.logger, $"Transaction UUID: {requestMessage.TransactionId}: PharmanetService.SubmitRequest end");
            return response;
        }
    }
}
