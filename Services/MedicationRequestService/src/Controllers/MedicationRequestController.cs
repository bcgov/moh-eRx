﻿//-------------------------------------------------------------------------
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
    [Route("/api/v{version:apiVersion}/MedicationRequest/")]
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
        /// The MedicationRequest is an order or request for both supply of the medication and the instructions for administration of
        /// the medication to a patient. The resource is called "MedicationRequest" rather than "MedicationPrescription" or "MedicationOrder"
        /// to generalize the use across inpatient and outpatient settings, including care plans, etc., and to harmonize with workflow patterns.
        /// </summary>
        /// <returns>A DocumentReference response as Json.</returns>
        /// <response code="200">Returns Ok when the transaction went through.</response>
        /// <response code="401">Authorization error, returns JSON describing the error.</response>
        /// <response code="500">The service error occurred.</response>
        [HttpPost]
        [Produces("application/fhir+json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Policy = FhirScopesPolicy.Access)]
        public async Task<ActionResult<DocumentReference>> MedicationRequest()
        {
            return await this.PharmanetRequest().ConfigureAwait(true);
        }
    }
}
