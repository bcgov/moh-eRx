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
    using System.Linq;
    using System.Threading.Tasks;

    using Health.PharmaNet.Common.Authorization.Claims;

    using Microsoft.AspNetCore.Authorization;

    /// <summary>
    /// PharmanetAuthorizationHandler resource-based handler to check for necessary scope claims.
    /// </summary>
    public class Hl7v2AuthorizationHandler : AuthorizationHandler<CorrectScopeRequirement, MessageType>
    {
        /// <summary>
        /// Checks if the user has the appropriate scopes claimed.
        /// </summary>
        /// <param name="context">The AuthorizationHandler context.</param>
        /// <param name="requirement">The authorization requirement being checked.</param>
        /// <param name="messageType">The Hl7-v2 MessageType of the request message.</param>
        /// <returns>A context Task.</returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CorrectScopeRequirement requirement, MessageType messageType)
        {
            // If user does not have the scope claim, get out of here
            if (!context.User.HasClaim(c => c.Type == PharmanetAPIClaims.Scope))
            {
                return Task.CompletedTask;
            }

            var scopeClaim = context.User.Claims.FirstOrDefault(
                    c => string.Equals(c.Type, PharmanetAPIClaims.Scope, StringComparison.OrdinalIgnoreCase));

            if (scopeClaim != null)
            {
                var scopes = scopeClaim.Value.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                foreach (string scope in scopes) {
                    if (requirement.HasCorrectScopeforMessageType(messageType, scope))
                    {
                        context.Succeed(requirement);
                        break;
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
