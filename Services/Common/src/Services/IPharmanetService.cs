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
    using System.Threading.Tasks;
    using Health.PharmaNet.Models;
    using Hl7.Fhir.Model;

    /// <summary>
    /// The Pharmanet service.
    /// </summary>
    public interface IPharmanetService
    {
        /// <summary>
        /// Submit request of type DocumentReference containing HL7v2 message to PharmaNet.
        /// </summary>
        /// <param name="request">An HL7 FHIR DocumentReference request containing HL7v2 payload.</param>
        /// <param name="traceId">The value used to track messages from API Gateway.</param>
        /// <returns>Returns a DocumentReference response.</returns>
        Task<RequestResult<DocumentReference>> SubmitRequest(DocumentReference request, string traceId);
    }
}