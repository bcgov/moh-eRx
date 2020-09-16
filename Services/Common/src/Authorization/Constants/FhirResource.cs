// -------------------------------------------------------------------------
//  Copyright © 2020 Province of British Columbia
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// -------------------------------------------------------------------------
namespace Health.PharmaNet.Common.Authorization.Constants
{
    /// <summary>
    /// A class with constants representing the various resource template names.
    /// </summary>
    public static class FhirResource
    {
        /// <summary>
        /// Wildcard representing all available resources under the given context.
        /// </summary>
        public const string Wildcard = "*";

        /// <summary>
        /// DocumentReference used to carry Hl7-v2 payload among other uses.
        /// See <a href="https://www.hl7.org/fhir/documentreference.html"/>.
        /// </summary>
        public const string DocumentReference = "DocumentReference";
    }
}
