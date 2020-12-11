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
    using System;
    using System.Net;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Class that represents the result of a request. Contains members for handling pagination and error resolution.
    /// </summary>
    /// <typeparam name="T">The payload type.</typeparam>
    public class RequestResult<T>
        where T : class
    {
        /// <summary>
        /// Gets or sets the result payload.
        /// </summary>
        [JsonPropertyName("Payload")]
        public T? Payload { get; set; } = default(T);

        /// <summary>
        /// Gets or sets a value indicating whether the status code is a success code.
        /// </summary>
        [JsonPropertyName("IsSuccessStatusCode")]
        public bool IsSuccessStatusCode { get; set; } = false;

        /// <summary>
        /// Gets or sets the status code of the result.
        /// </summary>
        [JsonPropertyName("StatusCode")]
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the status message of the result.
        /// </summary>
        [JsonPropertyName("ErrorMessage")]
        public string ErrorMessage { get; set; } = string.Empty;
    }
}