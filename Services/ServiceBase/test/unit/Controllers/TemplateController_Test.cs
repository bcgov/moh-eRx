namespace HealthGateway.Immunization.Test.Controller
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using Castle.Core.Logging;
    using DeepEqual.Syntax;
    using Health.ERX.Controllers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class TemplateController_Test
    {
        [Fact]
        public void ShouldOk()
        {
            Mock<IHttpContextAccessor> httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            TemplateController controller = new TemplateController(new Mock<ILogger<TemplateController>>().Object,httpContextAccessorMock.Object);
            ActionResult<string> result = controller.Test("123");
            Assert.True(true);
        }
    }
}