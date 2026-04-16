using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;

namespace BankAccountSimulation.Test.Helpers;

public abstract class TestBase
{
    protected Mock<IServiceProvider> MockServiceProvider = new();
    protected Mock<IAuthenticationService> MockAuthService = new();
    protected Mock<ITempDataDictionaryFactory> MockTempDataFactory = new();
    protected Mock<IUrlHelperFactory> MockUrlHelperFactory = new();

    protected void SetupControllerContext(Controller controller)
    {
        var httpContext = new DefaultHttpContext();

        MockServiceProvider
            .Setup(sp => sp.GetService(typeof(IAuthenticationService)))
            .Returns(MockAuthService.Object);

        MockServiceProvider
            .Setup(sp => sp.GetService(typeof(ITempDataDictionaryFactory)))
            .Returns(MockTempDataFactory.Object);

        MockTempDataFactory.Setup(f => f.GetTempData(It.IsAny<HttpContext>()))
            .Returns(new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>()));

        MockServiceProvider
            .Setup(sp => sp.GetService(typeof(IUrlHelperFactory)))
            .Returns(MockUrlHelperFactory.Object);

        httpContext.RequestServices = MockServiceProvider.Object;

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    protected void SetUserIdentity(Controller controller, string accountNumber, string role)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, accountNumber),
            new Claim(ClaimTypes.Role, role)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        controller.HttpContext.User = new ClaimsPrincipal(identity);
    }
}