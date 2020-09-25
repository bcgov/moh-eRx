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
namespace Health.PharmaNet.Models
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// The Pharmanet proxy message model. This json model is defined in collaboration with the vendor managing PharmaNet.
    /// </summary>
    public class PharmanetProxyMessage
    {
        /// <summary>
        /// Gets or sets the unique transaction identifier.
        /// </summary>
        [JsonPropertyName("transactionUUID")]
        public string TransactionId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Base 64 encoded HL7v2 Message.
        /// </summary>
        [JsonPropertyName("hl7Message")]
        public string Hl7Message { get; set; } = string.Empty;
    }
}
