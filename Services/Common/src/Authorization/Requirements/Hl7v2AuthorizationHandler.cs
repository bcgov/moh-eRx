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
    using System.Threading.Tasks;

    using Health.PharmaNet.Common.Authorization.Claims;
    using Health.PharmaNet.Common.Logging;
    using HL7.Dotnetcore;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// PharmanetAuthorizationHandler resource-based handler to check for necessary scope claims.
    /// </summary>
    public class Hl7v2AuthorizationHandler : AuthorizationHandler<Hl7v2AuthorizationRequirement, Message>
    {
        /// <summary>
        /// Gets or sets the Logger Service.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Hl7v2AuthorizationHandler"/> class.
        /// </summary>
        /// <param name="logger">Injected Logger Provider.</param>
        public Hl7v2AuthorizationHandler(ILogger<Hl7v2AuthorizationHandler> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Checks if the user has the appropriate scopes claimed for the given HL7-v2 MessageType.
        /// </summary>
        /// <param name="context">The AuthorizationHandler context.</param>
        /// <param name="requirement">The authorization requirement being checked.</param>
        /// <param name="resource">The Hl7-v2 Message of the request message.</param>
        /// <returns>A context Task.</returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, Hl7v2AuthorizationRequirement requirement, Message resource)
        {
            // Get information from the configuration about which segments to log
            IList<string> loggableSegments = requirement.LoggableSegments();
            string loggedMessage = "Logged HL7v2 message:";

            // Extract each matching segment of the message
            foreach (string segmentName in loggableSegments)
            {
                foreach (HL7.Dotnetcore.Segment segment in resource.Segments(segmentName))
                {
                    loggedMessage += " [ " + segment.Value + " ] " + System.Environment.NewLine;
                }
            }

            // Log the extracted segments of the message
            Logger.LogInformation(this.logger, loggedMessage);

            // If user does not have the scope claim, get out of here
            if (!context.User.HasClaim(c => c.Type == PharmanetAPIClaims.Scope))
            {
                Logger.LogError(this.logger, "Missing scope claim in JWT");
                return Task.CompletedTask;
            }

            var scopeClaim = context.User.Claims.FirstOrDefault(
                c => string.Equals(c.Type, PharmanetAPIClaims.Scope, StringComparison.OrdinalIgnoreCase));

            if (scopeClaim != null)
            {
                string[] scopes = scopeClaim.Value.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                string[] scopesNeeded = requirement.ScopesNeededForMessage(resource);

                if (HasCorrectScopes(scopesNeeded, scopes))
                {
                    Logger.LogInformation(this.logger, "HL7v2 Authorization Success! Scope(s) provided are correct for the HL7v2 message");
                    context.Succeed(requirement);
                }
                else
                {
                    if (scopesNeeded.Length > 0)
                    {
                        Logger.LogInformation(this.logger, "HL7v2 Authorization Failed! Scope(s) provided are NOT correct for the HL7v2 MessagType");
                    }
                    else
                    {
                        Logger.LogError(this.logger, "HL7v2 Authorization Failed! The HL7v2 Message is not known/supported by this service.");
                    }
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Returns whether any of the scopes provided is an accepted Scope for the given messagetype and optional control id.
        /// </summary>
        /// <param name="scopesNeeded">The scopes needed. The scopes passed in the JWT must match at least one.</param>
        /// <param name="scopes">The scopes claimed by User.</param>
        /// <returns>Returns true if the scopes provided matches any of the required scope values.</returns>
        private static bool HasCorrectScopes(string[] scopesNeeded, string[] scopes)
        {
            return scopes.Intersect<string>(scopesNeeded).Any<string>();
        }
    }
}
