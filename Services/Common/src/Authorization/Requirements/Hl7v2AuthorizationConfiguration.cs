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
    using System.Text.Json.Serialization;

    /// <summary>
    /// The configuration settings for determining Hl7v2 messages permitted for a given scope(s).
    /// </summary>
    public class Hl7v2AuthorizationConfiguration
    {
        /// <summary>
        /// Gets the Hl7v2 Hl7v2Authorization configurations.
        /// </summary>
        ///
        [JsonPropertyName("Hl7v2Authorization")]
        public Dictionary<string, Hl7v2Authorization> Hl7v2AuthorizationDict { get; } = new Dictionary<string, Hl7v2Authorization>();
    }
}