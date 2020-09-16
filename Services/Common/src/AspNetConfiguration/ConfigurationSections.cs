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
namespace Health.PharmaNet.Common.AspNetConfiguration
{
    /// <summary>
    /// The Configuration Sections.
    /// </summary>
    public static class ConfigurationSections
    {
        /// <summary>
        /// The Configuration Section for OAuth2 OpenIdConnect.
        /// </summary>
        public const string OpenIdConnect = "OpenIdConnect";

        /// <summary>
        /// The Configuration Section for OAuth2 Hl7-v2 Message Scopes by MessageType.
        /// </summary>
        public const string Hl7v2MessageScopes = "Hl7v2MessageScopes";
    }
}
