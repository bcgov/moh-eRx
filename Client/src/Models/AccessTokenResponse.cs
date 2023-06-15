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
namespace PharmaNet.Client.Models
{
    using System.Text.Json.Serialization;

    public partial class AccessTokenResponse
    {
        public string access_token { get; set; }

        public long expires_in { get; set; }

        public long refresh_expires_in { get; set; }

        public string token_type { get; set; }

        public string id_token { get; set; }

        [JsonPropertyName("not-before-policy")]
        public long not_before_policy { get; set; }

        public string scope { get; set; }
    }
}