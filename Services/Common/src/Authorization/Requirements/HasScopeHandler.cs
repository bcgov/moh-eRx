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
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Health.PharmaNet.Common.Authorization.Claims;
    using Health.PharmaNet.Common.Logging;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Authorization scope handler to check for necessary scope claims.
    /// </summary>
    public class HasScopeHandler : AuthorizationHandler<HasScopesRequirement>
    {
        /// <summary>
        /// Gets or sets the Logger Service.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="HasScopeHandler"/> class.
        /// </summary>
        /// <param name="logger">Injected Logger Provider.</param>
        public HasScopeHandler(ILogger<HasScopeHandler> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Checks if the user has the appropriate scopes claimed.
        /// </summary>
        /// <param name="context">The AuthorizationHandler context.</param>
        /// <param name="requirement">The authorization requirement being checked.</param>
        /// <returns>A context Task.</returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopesRequirement requirement)
        {
            if (context.User == null)
            {
                Logger.LogError(this.logger, "No authenticated user.");
                context.Fail();
                return Task.CompletedTask;
            }

            bool userIsAnonymous = context.User.Identity == null || !context.User.Identities.Any(i => i.IsAuthenticated);

            if (userIsAnonymous == true)
            {
                Logger.LogError(this.logger, "Skipping Authorization checks: user is not authenticated.");
                context.Fail();
                return Task.CompletedTask;
            }

            Claim? scopeClaim = context.User.Claims.FirstOrDefault<Claim>(c => string.Equals(c.Type, PharmanetAPIClaims.Scope, StringComparison.OrdinalIgnoreCase) && string.Equals(c.Issuer, requirement.ClaimsIssuer, StringComparison.OrdinalIgnoreCase));

            // If user does not have the scope claim, get out of here
            if (scopeClaim == null)
            {
                Logger.LogError(this.logger, "scopeClaim: Failed to find 'scope' claim in Access Token");
                return Task.CompletedTask;
            }

            // Split the scopes string into an array
            string[] scopes = scopeClaim.Value.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            // Succeed if the scope array contains any of the required scopes
            if (scopes.Any(s => requirement.IsRequiredScope(s) == true))
            {
                Logger.LogDebug(this.logger, "JWT Has at least one of the required scope claims.");
                context.Succeed(requirement);
            }
            else
            {
                Logger.LogError(this.logger, "JWT is missing the required scope claims.");
            }

            return Task.CompletedTask;
        }
    }
}
