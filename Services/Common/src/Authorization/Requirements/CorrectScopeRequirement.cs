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
    using System.Collections.Generic;
    using System;

    using Microsoft.AspNetCore.Authorization;

    /// <summary>
    /// object to pass as the TResource type for the handler.
    /// </summary>
    public class MessageType
    {
        /// <summary>
        /// Gets or sets the MessageType value.
        /// </summary>
        public string Value { get; set; } = string.Empty;
    }

    /// <summary>
    /// The Authorization Scope Requirement. The user (the jwt) must have at least one of the scopes specified.
    /// </summary>
    public class CorrectScopeRequirement : IAuthorizationRequirement
    {
        private Dictionary<string, string> authorizationDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="CorrectScopeRequirement"/> class.
        /// </summary>
        /// <param name="authorizationDictionary">The dictionary of scope+messageType pairings.</param>

        public CorrectScopeRequirement(Dictionary<string, string> authorizationDictionary)
        {
            this.authorizationDictionary = authorizationDictionary ??
                throw new ArgumentNullException(nameof(authorizationDictionary));
        }

        /// <summary>
        /// Returns whether the scope provided is an accepted Scope.
        /// </summary>
        /// <param name="messageType">The Hl7-v2 MessageType to be checked.</param>
        /// <param name="scope">The scope to be checked.</param>
        /// <returns>Returns true if the scope provided is the right one for the MessageType.</returns>
        public bool HasCorrectScopeforMessageType(MessageType messageType, string scope)
        {
            string scopeString = authorizationDictionary[messageType.Value];
            string[] scopes = scopeString.Split(' ');
            return Array.Exists(scopes, element => element == scope);
        }
    }
}
