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
namespace Health.PharmaNet.Common.Authorization
{
    using System;
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// The Authorization Scope Requirement. The user (the jwt) must have at least one of the scopes specified.
    /// </summary>
    public class Hl7v2AuthorizationRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// The Configuration Section for OAuth2 Hl7-v2 Message Scopes by MessageType.
        /// </summary>
        private const string Hl7v2AuthorizationConfigSection = "Hl7v2Authorization";

        private readonly Hl7v2AuthorizationConfiguration hl7AuthConfig;

        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="Hl7v2AuthorizationRequirement"/> class.
        /// </summary>
        /// <param name="configuration">The app setting configuration.</param>
        public Hl7v2AuthorizationRequirement(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.hl7AuthConfig = new Hl7v2AuthorizationConfiguration();
            this.configuration.Bind(Hl7v2AuthorizationConfigSection, this.hl7AuthConfig);
        }

        /// <summary>
        /// Returns whether the scope provided is an accepted Scope for the given messagetype and optional control id.
        /// </summary>
        /// <param name="messageType">The Hl7-v2 MessageType to be checked.</param>
        /// <param name="scope">The scope to be checked.</param>
        /// <returns>Returns true if the scope provided is the right one for the MessageType.</returns>
        public bool HasCorrectScopeforMessageType(MessageType messageType, string scope)
        {
            string key = messageType.Type + "|" + messageType.ControlId;

            if (this.hl7AuthConfig.Hl7v2Authorizations.TryGetValue(key, out Hl7v2Authorization? entry))
            {
                string[] scopes = entry.Scope!.Split(" ");
                return Array.Exists(scopes, element => element == scope);
            }

            return false;
        }
    }
}
