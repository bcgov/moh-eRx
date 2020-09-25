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
namespace Health.PharmaNet.Authorization.Policy
{
    /// <summary>
    /// The set of claims to access HL7 Scope based access to data.
    /// </summary>
    public static class Hl7v2ScopesPolicy
    {
        /// <summary>
        /// Policy which ensures that for a given HL7-v2 message type, the correct FHIR-based scope is present in the claims.
        /// </summary>
        public const string MessageTypeScopeAccess = "MessageTypeScopeAccess";
    }
}