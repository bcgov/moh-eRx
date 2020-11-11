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
    using System.Collections.Generic;
    using System.Linq;

    using Health.PharmaNet.Authorization.Requirements;
    using Health.PharmaNet.Authorization.Requirements.Models;

    using HL7.Dotnetcore;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

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
        /// Returns the configured scopes needed for the HL7v2 message passed in.
        /// </summary>
        /// <param name="message">The Hl7-v2 message to be checked.</param>
        /// <returns>Returns a string array of scopes needed, or empty array when the message was unknown/not supported.</returns>
        public string[] ScopesNeededForMessage(HL7.Dotnetcore.Message message)
        {
            return this.GetMessageAuthorizationScope(message);
        }

        /// <summary>
        /// Checks that the message passed has all the fields.
        /// </summary>
        /// <param name="messageConfig">The MessageConfig configuration from app settings.</param>
        /// <param name="message">The HL7v2 message being matched.</param>
        /// <returns>Returns true when the HL7v2 message was found in the configuration.</returns>
        private static bool IsConfiguredMessage(MessageConfig messageConfig, HL7.Dotnetcore.Message message)
        {
            int requiredMatches = messageConfig.MessageSegments.Count;
            int matches = 0;

            foreach (MessageSegment ms in messageConfig.MessageSegments)
            {
                List<string> segmentValues = new List<string>();

                foreach (HL7.Dotnetcore.Segment segment in message.Segments(ms.SegmentName))
                {
                    bool fieldsMatch = false;
                    bool firstField = true;

                    foreach (SegmentField sf in ms.SegmentFields)
                    {
                        bool found = segment.Fields(sf.Index).Value.Equals(sf.Value, StringComparison.Ordinal);
                        if (firstField == true)
                        {
                            fieldsMatch = found;
                            firstField = false;
                        }
                        else
                        {
                            fieldsMatch &= found;
                        }
                    }

                    if (fieldsMatch == true)
                    {
                        matches++;
                    }
                }
            }

            return matches == requiredMatches;
        }

        private static string GetMessageType(HL7.Dotnetcore.Message message)
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
        /// <returns>Returns the configured authorization scopes, or empty if unknown message.</returns>
        private string[] GetMessageAuthorizationScope(HL7.Dotnetcore.Message message)
        {
            string[] scopesNeeded = Array.Empty<string>();
            string messageType = GetMessageType(message); // MSH.9

            foreach (MessageConfig messageConfig in this.hl7AuthConfig.Hl7v2Authorization.MessageConfig)
            {
                if ((messageType == messageConfig.MessageType) && IsConfiguredMessage(messageConfig, message))
                {
                    scopesNeeded = messageConfig.Scope.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    break;
                }
            }

            return scopesNeeded;
        }
    }
}
