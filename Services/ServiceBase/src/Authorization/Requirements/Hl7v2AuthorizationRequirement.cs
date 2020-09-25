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
namespace Health.PharmaNet.Authorization
{
    using System;
    using System.Linq;

    using Health.PharmaNet.Authorization.Requirements;
    using Health.PharmaNet.Authorization.Requirements.Models;

    using HL7.Dotnetcore;

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
        private const string ConfigSection = "Hl7v2Authorization";

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
            this.configuration.Bind(ConfigSection, this.hl7AuthConfig.Hl7v2Authorization);
        }

        /// <summary>
        /// Returns whether any of the scopes provided is an accepted Scope for the given messagetype and optional control id.
        /// </summary>
        /// <param name="message">The Hl7-v2 message to be checked.</param>
        /// <param name="scopes">The scopes claimed by User.</param>
        /// <returns>Returns true if the scope provided is the right one for the MessageType.</returns>
        public bool HasCorrectScopesForMessage(Message message, string[] scopes)
        {
            string key = this.GetMessageAuthorizationKey(message);

            if (this.hl7AuthConfig.Hl7v2Authorization.MessageScopes.TryGetValue(key, out MessageScope? entry))
            {
                string[] scopesNeeded = entry.Scope!.Split(" ");

                return scopes.Intersect<string>(scopesNeeded).Any<string>();
            }

            return false;
        }

        private static string GetMessageType(Message message)
        {
            string messageType = string.Empty;

            if (message.IsComponentized("MSH.9"))
            {
                messageType = message.GetValue("MSH.9.1");
            }
            else
            {
                messageType = message.GetValue("MSH.9");
            }

            return messageType;
        }

        /// <summary>
        /// Determine the configuration authorization key to use to check authorization.
        /// </summary>
        /// <param name="message">An HL7v2 message.</param>
        /// <returns>Returns a MessageAuthorizationKey to use to lookup the configured authorization (scopes).</returns>
        private string GetMessageAuthorizationKey(Message message)
        {
            string key = string.Empty;
            string messageType = GetMessageType(message);

            if (this.hl7AuthConfig.Hl7v2Authorization.MessageTypeKeys.TryGetValue(messageType, out MessageTypeKey? messageKey))
            {
                key = messageKey.KeyTemplate;
                string[] segmentNames = messageKey.KeyTemplate.Split("_");

                foreach (string s in segmentNames)
                {
                    if (s.Equals("MSH.9", StringComparison.Ordinal))
                    {
                        key = key.Replace(s, messageType, StringComparison.Ordinal);
                    }
                    else
                    {
                        string segmentName = message.GetValue(s);
                        key = key.Replace(s, segmentName, StringComparison.Ordinal);
                    }
                }
            }

            return key;
        }
    }
}
