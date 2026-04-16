using BankAccountSimulation.Controllers;
using BankAccountSimulation.Domain.Entities;
using BankAccountSimulation.Domain.Enums;
using BankAccountSimulation.Models;
using BankAccountSimulation.Services.Interfaces;
using BankAccountSimulation.ViewModels;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Security.Claims;
using Xunit;

namespace BankAccountSimulation.Test.Controllers;

public class TransactionsControllerTests
{
    private readonly Mock<ITransactionsService> _mockService;
    private readonly Mock<IValidator<TransactionViewModel>> _mockTxValidator;
    private readonly Mock<IValidator<TransferViewModel>> _mockTransferValidator;
    private readonly TransactionsController _controller;

    public TransactionsControllerTests()
    {
        _mockService = new Mock<ITransactionsService>();
        _mockTxValidator = new Mock<IValidator<TransactionViewModel>>();
        _mockTransferValidator = new Mock<IValidator<TransferViewModel>>();

        _controller = new TransactionsController(
            _mockService.Object,
            _mockTxValidator.Object,
            _mockTransferValidator.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
            new(ClaimTypes.Name, "10112004")
        }, "TestAuth"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        _controller.TempData = new TempDataDictionary(_controller.HttpContext, Mock.Of<ITempDataProvider>());
    }

    #region GET Methods (Phủ 100% các hàm Get)
    [Fact]
    public void Deposit_Get_ReturnsView()
    {
        var result = _controller.Deposit("123");
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<TransactionViewModel>(viewResult.Model);
    }

    [Fact]
    public void Withdraw_Get_ReturnsView()
    {
        var result = _controller.Withdraw();
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Transfer_Get_ReturnsView()
    {
        var result = _controller.Transfer();
        Assert.IsType<ViewResult>(result);
    }
    #endregion

    #region POST Deposit
    [Fact]
    public async Task Deposit_ValidationFails_ReturnsViewWithErrors()
    {
        // Arrange
        var model = new TransactionViewModel();
        var failures = new List<ValidationFailure> {
            new("Amount", "Error 1"),
            new("AccountNumber", "Error 2")
        };
        _mockTxValidator.Setup(v => v.ValidateAsync(model, default)).ReturnsAsync(new ValidationResult(failures));

        // Act
        var result = await _controller.Deposit(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
        Assert.Equal(2, _controller.ModelState.ErrorCount);
    }

    [Fact]
    public async Task Deposit_ServiceFails_ReturnsViewWithErrorMessage()
    {
        // Arrange
        var model = new TransactionViewModel { AccountNumber = "123", Amount = 100 };
        _mockTxValidator.Setup(v => v.ValidateAsync(model, default)).ReturnsAsync(new ValidationResult());
        _mockService.Setup(s => s.ProcessTransaction(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<TransactionType>(), It.IsAny<string>()))
            .ReturnsAsync(OperationResult.Fail("Service Error"));

        // Act
        var result = await _controller.Deposit(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
    }
    #endregion

    #region POST Withdraw
    [Fact]
    public async Task Withdraw_Success_RedirectsToHistory()
    {
        // Arrange
        var model = new TransactionViewModel { AccountNumber = "123", Amount = 50, Description = "Atm" };
        _mockTxValidator.Setup(v => v.ValidateAsync(model, default)).ReturnsAsync(new ValidationResult());
        _mockService.Setup(s => s.ProcessTransaction("123", 50, TransactionType.Withdraw, "Atm"))
            .ReturnsAsync(OperationResult.Success());

        // Act
        var result = await _controller.Withdraw(model);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("History", redirect.ActionName);
    }
    #endregion

    #region POST Transfer
    [Fact]
    public async Task Transfer_ValidationFails_ReturnsView()
    {
        // Arrange
        var model = new TransferViewModel();
        _mockTransferValidator.Setup(v => v.ValidateAsync(model, default))
            .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("Dest", "Required") }));

        // Act
        var result = await _controller.Transfer(model);

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Transfer_Success_RedirectsToHistory()
    {
        // Arrange
        var model = new TransferViewModel { SourceAccountNumber = "A", DestinationAccountNumber = "B", Amount = 10 };
        _mockTransferValidator.Setup(v => v.ValidateAsync(model, default)).ReturnsAsync(new ValidationResult());
        _mockService.Setup(s => s.Transfer("A", "B", 10, It.IsAny<string>()))
            .ReturnsAsync(OperationResult.Success());

        // Act
        var result = await _controller.Transfer(model);

        // Assert
        Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Transfer success!", _controller.TempData["SuccessMessage"]);
    }
    #endregion

    #region History
    [Fact]
    public async Task History_Always_SetsViewBagAndReturnsView()
    {
        // Arrange
        _mockService.Setup(s => s.GetHistory(It.IsAny<string>(), "All", 1))
            .ReturnsAsync((new List<Transaction>(), 15));

        // Act
        var result = await _controller.History("All", 1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(2, _controller.ViewBag.TotalPages);
        Assert.Equal(15, _controller.ViewBag.TotalCount);
        Assert.Equal("10112004", _controller.ViewBag.AccountNumber);
    }
    #endregion
    #region Edge Cases & Deep Coverage

    [Fact]
    public async Task Deposit_DescriptionIsNull_UsesDefaultString()
    {
        // Arrange
        var model = new TransactionViewModel { AccountNumber = "123", Amount = 100, Description = null };
        _mockTxValidator.Setup(v => v.ValidateAsync(model, default)).ReturnsAsync(new ValidationResult());

        _mockService.Setup(s => s.ProcessTransaction("123", 100, TransactionType.Deposit, "Deposit Money"))
            .ReturnsAsync(OperationResult.Success());

        // Act
        var result = await _controller.Deposit(model);

        // Assert
        _mockService.Verify(s => s.ProcessTransaction("123", 100, TransactionType.Deposit, "Deposit Money"), Times.Once);
    }

    [Fact]
    public async Task Withdraw_DescriptionIsNull_UsesDefaultString()
    {
        // Arrange
        var model = new TransactionViewModel { AccountNumber = "123", Amount = 100, Description = null };
        _mockTxValidator.Setup(v => v.ValidateAsync(model, default)).ReturnsAsync(new ValidationResult());
        _mockService.Setup(s => s.ProcessTransaction("123", 100, TransactionType.Withdraw, "Withdraw money"))
            .ReturnsAsync(OperationResult.Success());

        // Act
        await _controller.Withdraw(model);

        // Assert
        _mockService.Verify(s => s.ProcessTransaction("123", 100, TransactionType.Withdraw, "Withdraw money"), Times.Once);
    }

    [Fact]
    public async Task History_TotalCountIsZero_ReturnsZeroPages()
    {
        // Arrange
        _mockService.Setup(s => s.GetHistory(It.IsAny<string>(), "All", 1))
            .ReturnsAsync((new List<Transaction>(), 0));

        // Act
        await _controller.History("All", 1);

        // Assert
        Assert.Equal(0, _controller.ViewBag.TotalPages);
        Assert.Equal(0, _controller.ViewBag.TotalCount);
    }

    [Fact]
    public async Task History_MultiplePages_CalculatesCorrectCeiling()
    {
        // Arrange
        _mockService.Setup(s => s.GetHistory(It.IsAny<string>(), "All", 1))
            .ReturnsAsync((new List<Transaction>(), 11));

        // Act
        await _controller.History("All", 1);

        // Assert
        Assert.Equal(2, _controller.ViewBag.TotalPages);
    }

    [Fact]
    public async Task Transfer_DescriptionProvided_PassesToService()
    {
        // Arrange
        var model = new TransferViewModel
        {
            SourceAccountNumber = "A",
            DestinationAccountNumber = "B",
            Amount = 10,
            Description = "Custom Memo"
        };
        _mockTransferValidator.Setup(v => v.ValidateAsync(model, default)).ReturnsAsync(new ValidationResult());
        _mockService.Setup(s => s.Transfer("A", "B", 10, "Custom Memo")).ReturnsAsync(OperationResult.Success());

        // Act
        await _controller.Transfer(model);

        // Assert
        _mockService.Verify(s => s.Transfer("A", "B", 10, "Custom Memo"), Times.Once);
    }
    #endregion
}