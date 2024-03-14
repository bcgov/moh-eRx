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
namespace Health.PharmaNet.Authorization.Requirements.Models
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Class representing the array of message authorizations needed for each Pharmanet Transaction.
    /// </summary>
    public class Hl7v2Authorization
    {
        /// <summary>
        /// Gets the list of segment fields that are allowed to be logged.
        /// </summary>
        [JsonPropertyName("LoggableSegments")]
        public IList<string> LoggableSegments { get; } = new List<string>();

        /// <summary>
        /// Gets the Messages configured.
        /// </summary>
        [JsonPropertyName("MessageConfig")]
        public IList<MessageConfig> MessageConfig { get; } = new List<MessageConfig>();
    }
}
