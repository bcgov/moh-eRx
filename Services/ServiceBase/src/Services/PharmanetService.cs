//-------------------------------------------------------------------------
// Copyright © 2019 Province of British Columbia
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
    using System.Threading.Tasks;

    using Health.PharmaNet.Delegates;
    using Health.PharmaNet.Models;

    using Hl7.Fhir.Model;

    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The PharmanetService.
    /// </summary>
    public class PharmanetService : IPharmanetService
    {
        private readonly IPharmanetDelegate pharmanetDelegate;

        private static PharmanetMessageModel convertFromFhir(DocumentReference documentReference)
        {
            return new PharmanetMessageModel(); // temp
        }
        private static DocumentReference convertToFhir(PharmanetMessageModel pharmanetMessageModel)
        {
            return new DocumentReference(); // temp
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PharmanetService"/> class.
        /// </summary>
        /// <param name="logger">Injected Logger Provider.</param>
        /// <param name="pharmanetDelegate">Injected pharmanet delegate.</param>
        public PharmanetService(ILogger<PharmanetService> logger,
            IPharmanetDelegate pharmanetDelegate)
        {
            this.pharmanetDelegate = pharmanetDelegate;
        }
        /// <summary>
        /// Submit Request to Pharmanet.
        ///</summary>
        public async Task<DocumentReference> SubmitRequest(DocumentReference request)
        {
            PharmanetMessageModel requestMessage =  convertFromFhir(request);

            PharmanetMessageModel responseMessage = await this.pharmanetDelegate.SubmitRequest(requestMessage);
            DocumentReference response = convertToFhir(responseMessage);
            return response;
        }
    }
}
