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
namespace Health.PharmaNet.Test
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using Castle.Core.Logging;
    using DeepEqual.Syntax;

    using Health.PharmaNet.Controllers;
    using Health.PharmaNet.Services;
    using Health.PharmaNet.Models;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class ServiceBaseControllerTest
    {
        [Fact]
        public void ShouldGetPharmanetResponse()
        {
           Mock<IPharmanetService> serviceMock = new Mock<IPharmanetService>();

        serviceMock.Setup(s => s.PharmanetRequest(It.IsAny<DocumentReference>())).Returns(new DocumentReference());

        Mock<ILogger> loggerMock = new Mock<ILogger>();

        string hl7v2 = "{Mock HL7v2}";

                    ILogger<ServiceBaseController> logger,
            IPharmanetService service,
            IHl7Parser parser,
            IConfiguration configuration,
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor)

        ServiceBaseController controller = new ServiceBaseController(
                loggerMock.Object, 
                serviceMock.Object,
                parser);


        }
    }
}