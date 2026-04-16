using BankAccountSimulation.Domain.Entities;
using BankAccountSimulation.Domain.Enums;
using BankAccountSimulation.Domain.Interfaces;
using BankAccountSimulation.Services;
using MockQueryable;
using Moq;
using System.Linq.Expressions;

public class TransactionsServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<IRepository<BankAccount>> _mockAccountRepo;
    private readonly Mock<IRepository<Transaction>> _mockTransRepo;
    private readonly TransactionsService _service;

    public TransactionsServiceTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockAccountRepo = new Mock<IRepository<BankAccount>>();
        _mockTransRepo = new Mock<IRepository<Transaction>>();

        _mockUow.Setup(u => u.Accounts).Returns(_mockAccountRepo.Object);
        _mockUow.Setup(u => u.Transactions).Returns(_mockTransRepo.Object);

        _service = new TransactionsService(_mockUow.Object);

        _mockUow.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _mockUow.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
        _mockUow.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);
    }

    [Fact]
    public async Task Transfer_Success_UpdatesBothAccounts()
    {
        // Arrange
        var source = new BankAccount { AccountNumber = "SRC", Balance = 500, Status = AccountStatus.Active };
        var dest = new BankAccount { AccountNumber = "DEST", Balance = 100, Status = AccountStatus.Active };

        _mockAccountRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<BankAccount, bool>>>()))
            .ReturnsAsync((Expression<Func<BankAccount, bool>> predicate) =>
            {
                var func = predicate.Compile();
                if (func(source)) return source;
                if (func(dest)) return dest;
                return null;
            });

        // Act
        var result = await _service.Transfer("SRC", "DEST", 200, "Transfer");

        // Assert
        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal(300, source.Balance);
        Assert.Equal(300, dest.Balance);

        _mockTransRepo.Verify(r => r.AddAsync(It.IsAny<Transaction>()), Times.Exactly(2));
    }

    [Fact]
    public async Task ProcessTransaction_Success_Commits()
    {
        // Arrange
        var account = new BankAccount { AccountNumber = "123", Balance = 1000, Status = AccountStatus.Active };

        _mockAccountRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<BankAccount, bool>>>()))
            .ReturnsAsync((Expression<Func<BankAccount, bool>> predicate) =>
            {
                var func = predicate.Compile();
                return func(account) ? account : null;
            });

        // Act
        var result = await _service.ProcessTransaction("123", 500, TransactionType.Deposit, "Bonus");

        // Assert
        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal(1500, account.Balance);
        _mockTransRepo.Verify(r => r.AddAsync(It.IsAny<Transaction>()), Times.Once);
    }

    [Fact]
    public async Task ProcessTransaction_Withdraw_Success()
    {
        // Arrange
        var account = new BankAccount { AccountNumber = "123", Balance = 1000, Status = AccountStatus.Active };

        _mockAccountRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<BankAccount, bool>>>()))
            .ReturnsAsync((Expression<Func<BankAccount, bool>> predicate) =>
            {
                var func = predicate.Compile();
                return func(account) ? account : null;
            });

        // Act
        var result = await _service.ProcessTransaction("123", 500, TransactionType.Withdraw, "Atm");

        // Assert
        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal(500, account.Balance);
    }

    [Fact]
    public async Task GetHistory_AdminRole_DoesNotFilterByAccountNumber()
    {
        // Arrange
        var admin = new BankAccount { AccountNumber = "Admin01", Role = "Admin", Status = AccountStatus.Active };
        var transactions = new List<Transaction>
        {
            new() { AccountNumber = "UserA", CreatedAt = DateTime.UtcNow },
            new() { AccountNumber = "UserB", CreatedAt = DateTime.UtcNow }
        }.BuildMock();

        _mockAccountRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<BankAccount, bool>>>())).ReturnsAsync(admin);
        _mockTransRepo.Setup(r => r.GetQueryable()).Returns(transactions);

        // Act
        var (items, count) = await _service.GetHistory("Admin01", "All", 1);

        // Assert
        Assert.Equal(2, count);
    }
}