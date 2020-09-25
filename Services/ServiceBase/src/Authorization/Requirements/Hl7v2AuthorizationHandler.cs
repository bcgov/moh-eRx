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
    using System.Threading.Tasks;

    using Health.PharmaNet.Common.Authorization.Claims;
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
            // If user does not have the scope claim, get out of here
            if (!context.User.HasClaim(c => c.Type == PharmanetAPIClaims.Scope))
            {
                this.logger.LogDebug("Missing scope claim in JWT");
                return Task.CompletedTask;
            }

            var scopeClaim = context.User.Claims.FirstOrDefault(
                c => string.Equals(c.Type, PharmanetAPIClaims.Scope, StringComparison.OrdinalIgnoreCase));

            if (scopeClaim != null)
            {
                string[] scopes = scopeClaim.Value.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                if (requirement.HasCorrectScopesForMessage(resource, scopes))
                {
                    this.logger.LogDebug("Scope(s) provided are correct for the HL7v2 MessagType");
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
