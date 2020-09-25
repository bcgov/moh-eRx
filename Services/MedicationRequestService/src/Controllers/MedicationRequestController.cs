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

    using Hl7.Fhir.Model;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The MedicationService controller.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("/api/v{version:apiVersion}/MedicationRequestService/")]
    [ApiController]
    public class MedicationRequestController : ServiceBaseController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MedicationRequestController"/> class.
        /// </summary>
        /// <param name="logger">Injected Logger Provider.</param>
        /// <param name="service">Injected service.</param>
        /// <param name="parser">Injected HL7 message parser.</param>
        /// <param name="configuration">Injected iConfiguration service.</param>
        /// <param name="authorizationService">Injected authorization service.</param>
        /// <param name="httpContextAccessor">The Http Context accessor.</param>
        public MedicationRequestController(
            ILogger<ServiceBaseController> logger,
            IPharmanetService service,
            IHl7Parser parser,
            IConfiguration configuration,
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor)
            : base(logger, service, parser, configuration, authorizationService, httpContextAccessor)
        {
        }

        /// <summary>
        /// The main controller.
        /// </summary>
        /// <returns>A DocumentReference response as Json.</returns>
        [HttpPost]
        [Produces("application/fhir+json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("MedicationRequest")]
        [Authorize(Policy = FhirScopesPolicy.Access)]
        public new async Task<ActionResult<DocumentReference>> PharmanetRequest()
        {
            return await base.PharmanetRequest().ConfigureAwait(false);
        }
    }
}
