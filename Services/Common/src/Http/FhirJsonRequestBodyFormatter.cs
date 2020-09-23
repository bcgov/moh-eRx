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
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.Formatters;
    using Microsoft.Net.Http.Headers;

    /// <summary>
    /// Formatter that allows content of type application/fhir+json
    /// or no content type to be parsed to raw data. Allows for a single input parameter
    /// in the form of:
    /// public string RawString([FromBody] string data).
    /// </summary>
    public class FhirJsonRequestBodyFormatter : InputFormatter
    {
        /// <summary>
        /// The media type or content type for this formatter.
        /// </summary>
        public const string MediaType = "application/fhir+json";

        /// <summary>
        /// Initializes a new instance of the <see cref="FhirJsonRequestBodyFormatter"/> class.
        /// </summary>
        public FhirJsonRequestBodyFormatter()
        {
            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue(MediaType));
        }

        /// <summary>
        /// Allow application/fhir+json and no content type to be processed.
        /// </summary>
        /// <param name="context">The InputFormatterContext.</param>
        /// <returns>Returns true if can read the context.</returns>
        public override bool CanRead(InputFormatterContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var contentType = context.HttpContext.Request.ContentType;
            if (string.IsNullOrEmpty(contentType) || contentType == MediaType)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Handle no content type or the application/fhir+json content type only.
        /// </summary>
        /// <param name="context">the InputFormatterContext input.</param>
        /// <returns>Task result.</returns>
        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var request = context.HttpContext.Request;
            var contentType = context.HttpContext.Request.ContentType;

            if (string.IsNullOrEmpty(contentType) || (contentType == MediaType))
            {
                using (var reader = new StreamReader(request.Body))
                {
                    var content = await reader.ReadToEndAsync().ConfigureAwait(false);
                    return await InputFormatterResult.SuccessAsync(content).ConfigureAwait(false);
                }
            }

            return await InputFormatterResult.FailureAsync().ConfigureAwait(false);
        }
    }
}