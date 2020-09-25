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
    using System.Text.Json.Serialization;

    /// <summary>
    /// The configuration settings for enforcing correct JWT for a given HL7v2 Message.
    /// </summary>
    public partial class Hl7v2Authorization
    {
        /// <summary>
        /// Gets or sets the Hl7v2 Message Type for authorization.
        /// </summary>
        [JsonPropertyName("MessageType")]
        public string MessageType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ControlId to use for authorization.
        /// </summary>
        [JsonPropertyName("ControlId")]
        public string ControlId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the scope to use for authorization.
        /// </summary>
        [JsonPropertyName("Scope")]
        public string Scope { get; set; } = string.Empty;
    }
}