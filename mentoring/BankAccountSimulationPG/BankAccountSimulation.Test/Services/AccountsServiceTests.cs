using BankAccountSimulation.Domain.Common;
using BankAccountSimulation.Domain.Entities;
using BankAccountSimulation.Domain.Enums;
using BankAccountSimulation.Domain.Interfaces;
using BankAccountSimulation.Services;
using BankAccountSimulation.ViewModels;
using Moq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using Xunit;

namespace BankAccountSimulation.Test.Services;

public class AccountsServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly AccountsService _service;

    public AccountsServiceTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _service = new AccountsService(_mockUow.Object);
    }

    // --- CreateAccount Tests ---

    [Fact]
    public async Task CreateAccount_AccountNumberAlreadyExists_ReturnsFailureResult()
    {
        // Arrange
        var request = new CreateAccountViewModel { AccountNumber = "123" };
        _mockUow.Setup(u => u.Accounts.FindAsync(It.IsAny<Expression<Func<BankAccount, bool>>>()))
            .ReturnsAsync(new BankAccount());

        // Act
        var result = await _service.CreateAccount(request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Account number already exists", result.ErrorMessage);
    }

    [Fact]
    public async Task CreateAccount_ValidRequest_ReturnsSuccessAndCallsSave()
    {
        // Arrange
        var request = new CreateAccountViewModel { AccountNumber = "999", Password = "123", InitialBalance = 100 };
        _mockUow.Setup(u => u.Accounts.FindAsync(It.IsAny<Expression<Func<BankAccount, bool>>>()))
            .ReturnsAsync((BankAccount)null!);

        // Act
        var result = await _service.CreateAccount(request);

        // Assert
        Assert.True(result.IsSuccess);
        _mockUow.Verify(u => u.Accounts.AddAsync(It.IsAny<BankAccount>()), Times.Once);
        _mockUow.Verify(u => u.CompleteAsync(), Times.Once);
    }

    // --- Login Tests ---

    [Fact]
    public async Task Login_AccountDoesNotExist_ReturnsFailureWithCorrectKey()
    {
        // Arrange
        _mockUow.Setup(u => u.Accounts.FindAsync(It.IsAny<Expression<Func<BankAccount, bool>>>()))
            .ReturnsAsync((BankAccount)null!);

        // Act
        var result = await _service.Login(new LoginAccountViewModel { AccountNumber = "NON_EXIST" });

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("AccountNumber", result.ErrorKey);
    }

    [Fact]
    public async Task Login_InvalidPassword_ReturnsFailureResult()
    {
        // Arrange
        var dbAccount = new BankAccount
        {
            PasswordHash = SecurityHelper.HashData<SHA256>("CorrectPass")
        };
        _mockUow.Setup(u => u.Accounts.FindAsync(It.IsAny<Expression<Func<BankAccount, bool>>>()))
            .ReturnsAsync(dbAccount);

        // Act
        var result = await _service.Login(new LoginAccountViewModel { Password = "WrongPass" });

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid password", result.ErrorMessage);
    }

    // --- UpdateStatus Tests ---

    [Fact]
    public async Task UpdateAccountStatus_AccountFound_UpdatesStatusAndSaves()
    {
        // Arrange
        var account = new BankAccount { Status = AccountStatus.Active };
        _mockUow.Setup(u => u.Accounts.FindAsync(It.IsAny<Expression<Func<BankAccount, bool>>>()))
            .ReturnsAsync(account);

        // Act
        await _service.UpdateAccountStatus("123", AccountStatus.Frozen);

        // Assert
        Assert.Equal(AccountStatus.Frozen, account.Status);
        _mockUow.Verify(u => u.CompleteAsync(), Times.Once);
    }

    // --- GetDetails Tests ---

    [Fact]
    public async Task GetAccountDetails_ExistingAccount_ReturnsCorrectViewModel()
    {
        // Arrange
        var account = new BankAccount { AccountNumber = "123", OwnerName = "Thanh" };
        _mockUow.Setup(u => u.Accounts.FindAsync(It.IsAny<Expression<Func<BankAccount, bool>>>()))
            .ReturnsAsync(account);

        // Act
        var result = await _service.GetAccountDetails("123");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Thanh", result!.OwnerName);
    }

    // --- GetAccountDetails Tests ---

    [Fact]
    public async Task GetAccountDetails_AccountDoesNotExist_ReturnsNull()
    {
        // Arrange
        _mockUow.Setup(u => u.Accounts.FindAsync(It.IsAny<Expression<Func<BankAccount, bool>>>()))
            .ReturnsAsync((BankAccount)null!);

        // Act
        var result = await _service.GetAccountDetails("NON_EXISTENT");

        // Assert
        Assert.Null(result);
    }

    // --- GetAllAccounts Tests ---

    [Fact]
    public async Task GetAllAccounts_WhenCalled_ReturnsAllAccountsMappedToViewModel()
    {
        // Arrange
        var dbAccounts = new List<BankAccount>
    {
        new() { AccountNumber = "1", OwnerName = "A", Status = AccountStatus.Active, CreatedAt = DateTime.Now },
        new() { AccountNumber = "2", OwnerName = "B", Status = AccountStatus.Frozen, CreatedAt = DateTime.Now }
    };
        _mockUow.Setup(u => u.Accounts.GetAllAsync()).ReturnsAsync(dbAccounts);

        // Act
        var result = await _service.GetAllAccounts();

        // Assert
        var list = result.ToList();
        Assert.Equal(2, list.Count);
        Assert.Equal("Active", list[0].Status);
        Assert.Equal("Frozen", list[1].Status);
    }

    // --- UpdateAccountStatus Tests ---

    [Fact]
    public async Task UpdateAccountStatus_AccountNotFound_ReturnsFailureResult()
    {
        // Arrange
        _mockUow.Setup(u => u.Accounts.FindAsync(It.IsAny<Expression<Func<BankAccount, bool>>>()))
            .ReturnsAsync((BankAccount)null!);

        // Act
        var result = await _service.UpdateAccountStatus("UNKNOWN", AccountStatus.Active);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Bank account not found.", result.ErrorMessage);
    }

    [Fact]
    public async Task UpdateAccountStatus_AccountExists_ReturnsSuccessAndUpdatesStatus()
    {
        // Arrange
        var account = new BankAccount { AccountNumber = "123", Status = AccountStatus.Active };
        _mockUow.Setup(u => u.Accounts.FindAsync(It.IsAny<Expression<Func<BankAccount, bool>>>()))
            .ReturnsAsync(account);

        // Act
        var result = await _service.UpdateAccountStatus("123", AccountStatus.Frozen);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(AccountStatus.Frozen, account.Status);
        _mockUow.Verify(u => u.CompleteAsync(), Times.Once);
    }
}