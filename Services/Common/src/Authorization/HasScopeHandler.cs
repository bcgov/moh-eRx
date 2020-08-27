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
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;

    /// <summary>
    /// Authorization scope handler to check for necessary scope claims.
    /// </summary>
    public class HasScopeHandler : AuthorizationHandler<HasScopesRequirement>
    {
        /// <summary>
        /// Checks if the user has the appropriate scopes claimed.
        /// </summary>
        /// <param name="context">The AuthorizationHandler context.</param>
        /// <param name="requirement">The authorization requirement being checked.</param>
        /// <returns>A context Task.</returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopesRequirement requirement)
        {
            // If user does not have the scope claim, get out of here
            if (!context.User.HasClaim(c => c.Type == "scope" && c.Issuer == requirement.ClaimsIssuer))
            {
                return Task.CompletedTask;
            }

            // Split the scopes string into an array
            var scopes = context.User.FindFirst(c => c.Type == "scope" && c.Issuer == requirement.ClaimsIssuer).Value.Split(' ');

            // Succeed if the scope array contains any of the required scopes
            if (scopes.Any(s => requirement.IsRequiredScope(s) == true))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
