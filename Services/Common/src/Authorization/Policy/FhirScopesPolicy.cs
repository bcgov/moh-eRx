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
namespace Health.PharmaNet.Common.Authorization.Policy
{
    /// <summary>
    /// The set of claims to access Fhir Scope based access to data.
    /// </summary>
    public static class FhirScopesPolicy
    {
        /// <summary>
        /// Policy which allows the access based on configured scopes allowed.
        /// </summary>
        public const string Access = "FhirScopesAccess";
    }
}