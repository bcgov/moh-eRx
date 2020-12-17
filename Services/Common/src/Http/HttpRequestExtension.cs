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
namespace Health.PharmaNet.Common.Http
{
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Extension to HttpRequest to deal with string  data type.
    /// </summary>
    public static class HttpRequestExtension
    {
        /// <summary>
        /// Retrieve the raw body as a string from the Request.Body stream.
        /// </summary>
        /// <param name="request">Request instance to apply to.</param>
        /// <param name="encoding">Optional - Encoding, defaults to UTF8.</param>
        /// <param name="inputStream">Optional - Pass in the stream to retrieve from. Other Request.Body.</param>
        /// <returns>The Request Body as a string.</returns>
        public static async Task<string> GetRawBodyStringAsync(this HttpRequest request, Encoding? encoding = null, Stream? inputStream = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            if (inputStream == null)
            {
                inputStream = request.Body;
            }

            using (StreamReader reader = new StreamReader(inputStream, encoding))
            {
                return await reader.ReadToEndAsync().ConfigureAwait(true);
            }
        }
    }
}