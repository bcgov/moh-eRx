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
namespace PharmaNet.Client.Services
{
    using System;

    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.IdentityModel.Tokens;
    using System.IO;

    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net.Http.Json;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;

    using PharmaNet.Client.Models;

    public class PostService : IPostService
    {
        private FHIRMessageFormat message;
        private string token;
        private string serviceUrl;

        private readonly HttpClient client = new HttpClient();

        public PostService(IConfiguration configuration, string token)
        {
            this.token = token;
            this.serviceUrl = configuration.GetValue<string>("ServiceUrl");

            // Create the properly formatted FHIR http content for the request
            message = FormatMessage(configuration.GetValue<string>("Payload"));
        }

        public async Task<string> PostRequest() {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, serviceUrl);

                // Load the content into the request
                request.Content = JsonContent.Create<FHIRMessageFormat>(message);

                // Add the request headers
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.token);

                HttpResponseMessage response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode();

                return response.ToString();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error Message {e.Message}");
            }

            return string.Empty;
        }

        public FHIRMessageFormat FormatMessage(string payload) {
            UTF8Encoding utf8 = new UTF8Encoding();

            // Encode the payload in base64 as per HL7-v2 specifications
            Byte[] encodedBytes = utf8.GetBytes(payload);
            string encodedPayload = Convert.ToBase64String(encodedBytes);

            return new FHIRMessageFormat {
                resourceType = "DocumentReference",

                masterIdentifier = new Dictionary<string, string> {
                    ["system"] = "urn:ietf:rfc:3986",
                    ["value"] = System.Guid.NewGuid().ToString()
                },

                status = "current",

                date = DateTime.Now.ToString("s") + "+00:00",

                content = new List<Dictionary<string, Dictionary<string, string>>>() {
                    new Dictionary<string, Dictionary<string, string>> {
                        ["attachment"] = new Dictionary<string, string> {
                            ["contentType"] = "x-application/hl7-v2+er7",
                            ["data"] = encodedPayload
                        }
                    }
                }
            };
        }

    }
}