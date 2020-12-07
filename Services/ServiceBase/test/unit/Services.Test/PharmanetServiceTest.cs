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
    using System.Net;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using Castle.Core.Logging;
    using DeepEqual.Syntax;

    using Health.PharmaNet.Controllers;
    using Health.PharmaNet.Delegates;
    using Health.PharmaNet.Models;
    using Health.PharmaNet.Services;

    using Hl7.Fhir.Model;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class PharmanetServiceTest
    {
        [Setup]
        public void Setup()
        {

        }
        
        [Fact]
        public void ValidatePharmanetResponse()
        {
            var mockDelegate = new Mock<IPharmanetDelegate>();
            RequestResult<DocumentReference> delegateResult = new RequestResult<DocumentReference>()
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccessStatusCode = true,
                Payload = new DocumentReference()
                {
                },
            };

        }
    }
}