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
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Health.PharmaNet.Common.Authorization.Policy;
    using Health.PharmaNet.Common.Http;
    using Health.PharmaNet.Parsers;
    using Health.PharmaNet.Services;

    using HL7.Dotnetcore;
    using Hl7.Fhir.Model;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
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
            DocumentReference request = this.parser.ParseFhirJson(jsonString);

            Message message = this.ExtractV2Message(request);

            AuthorizationResult result = await this.authorizationService.AuthorizeAsync(
                    user,
                    message,
                    FhirScopesPolicy.MessageTypeScopeAccess).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                return new ChallengeResult();
            }

            DocumentReference response = await this.service.SubmitRequest(request).ConfigureAwait(false);
            return response;
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
