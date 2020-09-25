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
namespace Health.PharmaNet.Parsers
{
    using HL7.Dotnetcore;
    using Hl7.Fhir.Model;

    /// <summary>
    /// The Pharmanet service.
    /// </summary>
    public interface IHl7Parser
    {
        /// <summary>
        /// Parse a string containing HL7v2 message.
        /// </summary>
        /// <param name="messageString">An HL7v2 message as a string.</param>
        /// <returns>Returns an HL7v2 Message response.</returns>
        Message ParseV2String(string messageString);

        /// <summary>
        /// Parse a string containing FHIR message.
        /// </summary>
        /// <param name="json">An HL7 FHIR message as a string.</param>
        /// <returns>Returns a FHIR DocumentReference.</returns>
        DocumentReference ParseFhirJson(string json);
    }
}