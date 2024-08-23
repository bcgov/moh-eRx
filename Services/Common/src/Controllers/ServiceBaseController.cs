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
    using System;

    using System.Linq;
    using System.Net;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Health.PharmaNet.Authorization.Policy;
    using Health.PharmaNet.Common.Http;
    using Health.PharmaNet.Common.Logging;
    using Health.PharmaNet.Models;
    using Health.PharmaNet.Parsers;
    using Health.PharmaNet.Services;

    using HL7.Dotnetcore;
    using Hl7.Fhir.Model;
    using Hl7.Fhir.Serialization;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The Template controller.
    /// </summary>
    // ApiVersion("1.0")]
    // [Route("/api/v{version:apiVersion}/ServiceBase/")]
    // [ApiController]
    public class ServiceBaseController : ControllerBase
    {
        /// <summary>
        /// The Logger Service.
        /// </summary>
        private readonly ILogger logger;
        private readonly IConfiguration configuration;

        /// <summary>
        /// The IPharmanet Service.
        /// </summary>
        private readonly IPharmanetService service;

        /// <summary>
        /// The http context accessor.
        /// </summary>
        private readonly IHttpContextAccessor httpContextAccessor;

        /// <summary>
        /// The authorization service.
        /// </summary>
        private readonly IAuthorizationService authorizationService;

        /// <summary>
        /// The HL7 parser service.
        /// </summary>
        private readonly IHl7Parser parser;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBaseController"/> class.
        /// </summary>
        /// <param name="logger">Injected Logger Provider.</param>
        /// <param name="service">Injected service.</param>
        /// <param name="parser">Injected HL7 message parser.</param>
        /// <param name="configuration">Injected iConfiguration service.</param>
        /// <param name="authorizationService">Injected authorization service.</param>
        /// <param name="httpContextAccessor">The Http Context accessor.</param>
        public ServiceBaseController(
            ILogger<ServiceBaseController> logger,
            IPharmanetService service,
            IHl7Parser parser,
            IConfiguration configuration,
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor)
        {
            this.service = service;
            this.logger = logger;
            this.parser = parser;
            this.configuration = configuration;
            this.authorizationService = authorizationService;
            this.httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Takes request body containing an HL7 FHIR DocumentReference Object containing HL7v2 payload.
        /// </summary>
        /// <returns>The DocumentReference Response, or error JSON.</returns>
        /// <response code="200">Returns Ok when the transaction went through.</response>
        /// <response code="401">Authorization error, returns JSON describing the error.</response>
        /// <response code="50x">The service is unavailable for use.</response>
        // [HttpPost]
        // [Route("DocumentReference")]
        // [Produces("application/fhir+json")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [Authorize(Policy = FhirScopesPolicy.Access)]
        [Authorize]
        protected async Task<ActionResult<DocumentReference>> PharmanetRequest()
        {
            Logger.LogInformation(this.logger, $"ServiceBaseController.PharmanetRequest start");

            ClaimsPrincipal? user = this.HttpContext!.User;

            var traceId = this.Request.Headers.TryGetValue("Kong-Request-ID", out var value) ? value.FirstOrDefault() : "";
            Logger.LogInformation(this.logger, $"Trace ID: {traceId}: ServiceBaseController.PharmanetRequest. Extracted Kong-Request-ID header as the Trace ID.");

            string jsonString = await this.Request.GetRawBodyStringAsync().ConfigureAwait(true);

            Logger.LogInformation(this.logger, $"Trace ID: {traceId}: ServiceBaseController.PharmanetRequest: Got the body from the request.");

            DocumentReference fhirRequest;
            Message hl7v2Message;
            try
            {
                fhirRequest = this.parser.ParseFhirJson(jsonString);
                hl7v2Message = this.ExtractV2Message(fhirRequest);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                // Return Bad Message Http Error since the HL7 payload could not be parsed.
                Logger.LogException(this.logger, "PharmanetRequest() Bad Request", ex);
                return this.StatusCode((int)HttpStatusCode.BadRequest, ex.Message);
            }

            HL7.Dotnetcore.Segment? mshSegment = hl7v2Message.Segments("MSH").FirstOrDefault();
            Logger.LogInformation(this.logger, $"Trace ID: {traceId}: ServiceBaseController.PharmanetRequest: Message MSH: {mshSegment?.Value}");

            Logger.LogInformation(this.logger, $"Trace ID: {traceId}: ServiceBaseController.PharmanetRequest: Authorizing...");
            AuthorizationResult authResult = await this.authorizationService.AuthorizeAsync(
                    user,
                    hl7v2Message,
                    Hl7v2ScopesPolicy.MessageTypeScopeAccess).ConfigureAwait(true);
            if (!authResult.Succeeded)
            {
                return new ChallengeResult();
            }
            Logger.LogInformation(this.logger, $"Trace ID: {traceId}: ServiceBaseController.PharmanetRequest: Authorization completed. Submitting request...");

            RequestResult<DocumentReference> response = await this.service.SubmitRequest(fhirRequest, traceId + "").ConfigureAwait(true);
            if (response.IsSuccessStatusCode == false)
            {
                Logger.LogError(this.logger, $"An Error occurred while invoking Pharmanet endpoint: {response.ErrorMessage}");
                this.StatusCode((int)response.StatusCode);
                return new JsonResult(response)
                {
                    StatusCode = (int)response.StatusCode,
                    ContentType = "application/json",
                };
            }
            Logger.LogInformation(this.logger, $"Trace ID: {traceId}: ServiceBaseController.PharmanetRequest: Request completed.");

            DocumentReference? docRef = response.Payload;

            FhirJsonSerializer serializer = new FhirJsonSerializer(new SerializerSettings() { Pretty = true });

            Logger.LogInformation(this.logger, $"Trace ID: {traceId}: ServiceBaseController.PharmanetRequest end");
            return new ContentResult()
            {
                Content = serializer.SerializeToString(docRef),
                ContentType = "application/fhir+json; charset=utf-8",
            };
        }

        private Message ExtractV2Message(DocumentReference request)
        {
            DocumentReference.ContentComponent[] content = request.Content.ToArray();

            byte[] data = content[0].Attachment.Data; // The data is returned decoded from Base64 original encoding.
            string? msgString = System.Text.Encoding.UTF8.GetString(data, 0, data.Length);

            Message message = this.parser.ParseV2String(msgString);

            return message;
        }
    }
}
