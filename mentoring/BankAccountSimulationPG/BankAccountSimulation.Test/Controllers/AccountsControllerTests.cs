using BankAccountSimulation.Controllers;
using BankAccountSimulation.Domain.Enums;
using BankAccountSimulation.Models;
using BankAccountSimulation.Services.Interfaces;
using BankAccountSimulation.Test.Helpers;
using BankAccountSimulation.ViewModels;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace BankAccountSimulation.Test.Controllers;

public class AccountsControllerTests : TestBase
{
    private readonly Mock<IAccountsService> _mockAccountService;
    private readonly Mock<IValidator<CreateAccountViewModel>> _mockCreateValidator;
    private readonly Mock<IValidator<LoginAccountViewModel>> _mockLoginValidator;
    private readonly AccountsController _controller;

    public AccountsControllerTests()
    {
        _mockAccountService = new Mock<IAccountsService>();
        _mockCreateValidator = new Mock<IValidator<CreateAccountViewModel>>();
        _mockLoginValidator = new Mock<IValidator<LoginAccountViewModel>>();

        _controller = new AccountsController(
            _mockCreateValidator.Object,
            _mockLoginValidator.Object,
            _mockAccountService.Object
        );

        SetupControllerContext(_controller);
    }

    [Fact]
    public async Task Index_UserIsAdmin_ReturnsViewWithAccounts()
    {
        // Arrange
        var mockAccounts = new List<AccountsViewModel>
        {
            new() { AccountNumber = "123", OwnerName = "Thanh", Balance = 1000 }
        };
        _mockAccountService.Setup(s => s.GetAllAccounts()).ReturnsAsync(mockAccounts);

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<AccountsViewModel>>(viewResult.ViewData.Model);
        Assert.Single(model);
    }

    [Fact]
    public async Task Create_Post_InvalidValidation_ReturnsViewWithModel()
    {
        // Arrange
        var request = new CreateAccountViewModel { AccountNumber = "" };
        var validationFailures = new List<ValidationFailure> { new("AccountNumber", "Required") };
        _mockCreateValidator.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult(validationFailures));

        // Act
        var result = await _controller.Create(request);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
        Assert.Equal(request, viewResult.Model);
    }

    [Fact]
    public async Task Login_Post_Success_SetsCookieAndRedirects()
    {
        // Arrange
        var request = new LoginAccountViewModel { AccountNumber = "123456", Password = "password" };
        var accountDetail = new AccountsViewModel { AccountNumber = "123456", Role = "User" };

        _mockLoginValidator.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult());

        _mockAccountService.Setup(s => s.Login(request))
            .ReturnsAsync(OperationResult.Success());

        _mockAccountService.Setup(s => s.GetAccountDetails(request.AccountNumber))
            .ReturnsAsync(accountDetail);

        // Act
        var result = await _controller.Login(request);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Home", redirect.ControllerName);

        MockAuthService.Verify(a => a.SignInAsync(
            It.IsAny<HttpContext>(),
            It.IsAny<string>(),
            It.IsAny<ClaimsPrincipal>(),
            It.IsAny<AuthenticationProperties>()), Times.Once);
    }

    [Fact]
    public async Task Details_AccountNotFound_ReturnsNotFound()
    {
        // Arrange
        SetUserIdentity(_controller, "999", "User");
        _mockAccountService.Setup(s => s.GetAccountDetails("999"))
            .ReturnsAsync((AccountsViewModel)null!);

        // Act
        var result = await _controller.Details();

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Freeze_AdminUser_RedirectsToIndex()
    {
        // Arrange
        string accNum = "123456";
        SetUserIdentity(_controller, "admin_acc", "Admin"); 

        // Act
        var result = await _controller.Freeze(accNum);

        // Assert
        _mockAccountService.Verify(s => s.UpdateAccountStatus(accNum, AccountStatus.Frozen), Times.Once);
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }

    [Fact]
    public async Task Freeze_NormalUser_RedirectsToDetails()
    {
        // Arrange
        string accNum = "123456";
        SetUserIdentity(_controller, "user_acc", "User"); 

        // Act
        var result = await _controller.Freeze(accNum);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirect.ActionName);
    }

    [Fact]
    public void Login_Get_ReturnsView()
    {
        var result = _controller.Login();
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Create_Get_ReturnsView()
    {
        var result = _controller.Create();
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Index_ServiceReturnsNull_ReturnsViewWithEmptyList()
    {
        // Arrange
        _mockAccountService.Setup(s => s.GetAllAccounts())
            .ReturnsAsync((List<AccountsViewModel>)null!);

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<AccountsViewModel>>(viewResult.Model);
        Assert.Empty(model);
    }

    [Fact]
    public async Task Unfreeze_AdminUser_RedirectsToIndex()
    {
        // Arrange
        SetUserIdentity(_controller, "admin_user", "Admin");
        string accNum = "123456";

        // Act
        var result = await _controller.Unfreeze(accNum);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        _mockAccountService.Verify(s => s.UpdateAccountStatus(accNum, AccountStatus.Active), Times.Once);
    }

    [Fact]
    public async Task Unfreeze_NormalUser_RedirectsToDetails()
    {
        // Arrange
        SetUserIdentity(_controller, "normal_user", "User");
        string accNum = "123456";

        // Act
        var result = await _controller.Unfreeze(accNum);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirect.ActionName);
    }

    [Fact]
    public async Task Create_Post_ServiceReturnsError_ReturnsViewWithModelStateError()
    {
        // Arrange
        var request = new CreateAccountViewModel { AccountNumber = "111" };
        _mockCreateValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(new ValidationResult());
        _mockAccountService.Setup(s => s.CreateAccount(request))
            .ReturnsAsync(OperationResult.Fail("Duplicate Account", "AccountNumber"));

        // Act
        var result = await _controller.Create(request);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
        Assert.Equal("Duplicate Account", _controller.ModelState["AccountNumber"]?.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task Login_Post_InvalidPassword_ReturnsViewWithErrorMessage()
    {
        // Arrange
        var request = new LoginAccountViewModel { AccountNumber = "123", Password = "wrong" };
        _mockLoginValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(new ValidationResult());
        _mockAccountService.Setup(s => s.Login(request))
            .ReturnsAsync(OperationResult.Fail("Invalid password", "Password"));

        // Act
        var result = await _controller.Login(request);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
        Assert.Equal("Invalid password", _controller.ModelState["Password"]?.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task Details_UserIsAdmin_ReturnsAllAccounts()
    {
        // Arrange
        SetUserIdentity(_controller, "admin_user", "Admin");
        var currentAcc = new AccountsViewModel { AccountNumber = "admin_user", Role = "Admin" };
        var allAccs = new List<AccountsViewModel> { currentAcc, new() { AccountNumber = "user1" } };

        _mockAccountService.Setup(s => s.GetAccountDetails("admin_user")).ReturnsAsync(currentAcc);
        _mockAccountService.Setup(s => s.GetAllAccounts()).ReturnsAsync(allAccs);

        // Act
        var result = await _controller.Details();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<AccountsViewModel>>(viewResult.Model);
        Assert.Equal(2, (model as List<AccountsViewModel>)?.Count);
    }

    [Fact]
    public async Task Logout_Post_SignsOutAndRedirects()
    {
        // Arrange & Act
        var result = await _controller.Logout();

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Login", redirect.ActionName);
        MockAuthService.Verify(a => a.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()), Times.Once);
    }

}