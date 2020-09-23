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
namespace Health.PharmaNet.Controllers
{
    using System.IO;
    using System.Security.Claims;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Health.PharmaNet.Common.Authorization;
    using Health.PharmaNet.Common.Authorization.Policy;
    using Health.PharmaNet.Common.Http;
    using Health.PharmaNet.Parsers;
    using Health.PharmaNet.Services;

    using Hl7.Fhir.Model;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The Template controller.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("/api/v{version:apiVersion}/MedicationService/")]
    [ApiController]
    public class ServiceBaseController : ControllerBase
    {
        /// <summary>
        /// Gets or sets the Logger Service.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Gets or sets the MedicationRequest Service.
        /// </summary>
        private readonly IPharmanetService service;

        /// <summary>
        /// Gets or sets the http context accessor.
        /// </summary>
        private readonly IHttpContextAccessor httpContextAccessor;

        /// <summary>
        /// Gets or sets the authorization service.
        /// </summary>
        private readonly IAuthorizationService authorizationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBaseController"/> class.
        /// </summary>
        /// <param name="logger">Injected Logger Provider.</param>
        /// <param name="service">Injected service.</param>
        /// <param name="authorizationService">Injected authorization service.</param>
        /// <param name="httpContextAccessor">The Http Context accessor.</param>
        public ServiceBaseController(
            ILogger<ServiceBaseController> logger,
            IPharmanetService service,
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor)
        {
            this.service = service;
            this.logger = logger;
            this.authorizationService = authorizationService;
            this.httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Takes request body containing an HL7 FHIR DocumentReference Object containing HL7v2 payload.
        /// </summary>
        /// <returns>The DocumentReference Response, or error JSON.</returns>
        /// <response code="200">Returns Ok when the transaction went through.</response>
        /// <response code="401">Authorization error, returns JSON describing the error.</response>
        /// <response code="503">The service is unavailable for use.</response>
        [HttpPost]
        [Produces("application/fhir+json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("MedicationRequest")]
        [Authorize(Policy = FhirScopesPolicy.Access)]
        public async Task<ActionResult<DocumentReference>> PharmanetRequest()
        {
            ClaimsPrincipal user = this.httpContextAccessor.HttpContext.User;
            string accessToken = await this.httpContextAccessor.HttpContext.GetTokenAsync("access_token").ConfigureAwait(true);

            string jsonString = await this.Request.GetRawBodyStringAsync().ConfigureAwait(false);
            DocumentReference request = Hl7FhirParser.ParseJson(jsonString);

            MessageType messageType = GetHl7v2MessageType(request);

            AuthorizationResult result = await AuthorizationServiceExtensions.AuthorizeAsync(
                    this.authorizationService,
                    user,
                    FhirScopesPolicy.MessageTypeScopeAccess).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                return new ChallengeResult();
            }

            DocumentReference response = await this.service.SubmitRequest(request).ConfigureAwait(false);
            return response;
        }

        /// <summary>
        /// Parses the Json into an HL7 FHIR DocumentReference object.
        /// </summary>
        /// <param name="json">The JSON to parse.</param>
        /// <returns>DocumentReference instance or throws an error.</returns>
        private static DocumentReference ParseJsonBody(string json)
        {
            return Hl7FhirParser.ParseJson(json);
        }

        private static MessageType GetHl7v2MessageType(DocumentReference request)
        {
            DocumentReference.ContentComponent[] content = request.Content.ToArray();

            byte[] data = content[0].Attachment.Data;
            string? base64string = System.Text.Encoding.UTF8.GetString(data, 0, data.Length);

            HL7.Dotnetcore.Message msg = HL7v2Parser.ParseBase64EncodedData(base64string);

            return new MessageType(HL7v2Parser.GetMessageType(msg));
        }

        private static string ToJsonString(JsonDocument jsonDocument)
        {
            using (var stream = new MemoryStream())
            {
                Utf8JsonWriter writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });
                jsonDocument.WriteTo(writer);
                writer.Flush();
                writer.Dispose();
                return System.Text.Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }
}
