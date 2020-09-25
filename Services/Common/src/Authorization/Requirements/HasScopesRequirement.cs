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

    /// <summary>
    /// The Authorization Scope Requirement. The user (the jwt) must have at least one of the scopes specified.
    /// </summary>
    public class HasScopesRequirement : IAuthorizationRequirement
    {
        private string[] scope;

        /// <summary>
        /// Initializes a new instance of the <see cref="HasScopesRequirement"/> class.
        /// </summary>
        /// <param name="claimsIssuer">The ClaimsIssuer.</param>
        /// <param name="scope">The array of acceptable scopes for this service. User must have at least one of these in JWT.</param>
        public HasScopesRequirement(string[] scope, string claimsIssuer)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.ClaimsIssuer = claimsIssuer ?? throw new ArgumentNullException(nameof(claimsIssuer));
        }

        /// <summary>
        /// Gets the ClaimsIssuer.
        /// </summary>
        public string ClaimsIssuer { get; }

        /// <summary>
        /// Returns whether the scope provided is an accepted Scope.
        /// </summary>
        /// <param name="scope">The scope to be checked.</param>
        /// <returns>Returns true if the scope provided is in the array of acceptable scopes.</returns>
        public bool IsRequiredScope(string scope)
        {
            return Array.Exists(this.scope, element => element == scope);
        }
    }
}
