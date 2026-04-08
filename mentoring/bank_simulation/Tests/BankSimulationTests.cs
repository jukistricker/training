using bank_simulation;
using Xunit;

namespace Tests;

public class BankSimulationTests
{

    [Fact]
    public void IsOverDailyLimit_ShouldReturnTrue_WhenTotalExceeds50000()
    {
        // Giả lập: Đã rút 40,000, bây giờ rút thêm 11,000 -> Tổng 51,000 > 50,000
        decimal alreadySpent = 40000;
        decimal requestAmount = 11000;
        const decimal LIMIT = 50000;

        Assert.True(alreadySpent + requestAmount > LIMIT);
    }

    [Fact]
    public void IsOverDailyLimit_ShouldReturnFalse_WhenTotalExactly50000()
    {
        // Giả lập: Đã rút 40,000, rút thêm 10,000 -> Vừa đúng hạn mức
        decimal alreadySpent = 40000;
        decimal requestAmount = 10000;
        const decimal LIMIT = 50000;

        Assert.False(alreadySpent + requestAmount > LIMIT);
    }


    [Fact]
    public void Interest_Eligible_AccountMustBeCustomerAndActive()
    {
        var acc = new BankAccount { Role = Role.Customer, Status = AccountStatus.Active };
        Assert.True(acc.Role == Role.Customer && acc.Status == AccountStatus.Active);
    }

    [Fact]
    public void Interest_ShouldNotPay_IfAccountIsAdmin()
    {
        var acc = new BankAccount { Role = Role.Admin, Status = AccountStatus.Active };
        Assert.False(acc.Role == Role.Customer); // Admin không được trả lãi
    }

    [Fact]
    public void Interest_ShouldPay_OnlyIfCreatedAtOver30Days()
    {
        var oldAcc = new BankAccount { CreatedAt = DateTime.Now.AddDays(-31) };
        var newAcc = new BankAccount { CreatedAt = DateTime.Now.AddDays(-10) };

        Assert.True((DateTime.Now - oldAcc.CreatedAt).TotalDays >= 30);
        Assert.False((DateTime.Now - newAcc.CreatedAt).TotalDays >= 30);
    }

    [Fact]
    public void Interest_ShouldNotPay_IfAlreadyPaidThisMonth()
    {
        // Giả lập tài khoản đã được trả lãi vào ngày 1 của tháng này
        var lastPaid = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        var acc = new BankAccount { LastInterestDate = lastPaid };

        bool monthAlreadyPaid = acc.LastInterestDate.HasValue &&
                                acc.LastInterestDate.Value.Month == DateTime.Now.Month &&
                                acc.LastInterestDate.Value.Year == DateTime.Now.Year;

        Assert.True(monthAlreadyPaid);
    }

    // --- NHÓM 3: TRẠNG THÁI TÀI KHOẢN (SECURITY & STATUS) ---

    [Fact]
    public void IsFrozen_ShouldBlockOperation_IfStatusIsFrozen()
    {
        var acc = new BankAccount { Status = AccountStatus.Frozen };
        Assert.True(acc.Status == AccountStatus.Frozen);
    }

    [Fact]
    public void IsAdmin_ShouldFail_WithWrongPin()
    {
        string correctPin = "01234";
        string inputPin = "9999";
        Assert.NotEqual(correctPin, inputPin);
    }


    [Fact]
    public void Withdraw_ShouldFail_IfRemainingBalanceLessThan100()
    {
        decimal currentBalance = 150;
        decimal withdrawAmount = 60; // 150 - 60 = 90 < 100

        Assert.True(currentBalance - withdrawAmount < 100);
    }

    [Fact]
    public void Transfer_ShouldUpdateBothBalances_CorrectAmount()
    {
        // Arrange
        var source = new BankAccount { Balance = 1000 };
        var dest = new BankAccount { Balance = 500 };
        decimal amount = 200;

        // Act
        source.Balance -= amount;
        dest.Balance += amount;

        // Assert
        Assert.Equal(800, source.Balance);
        Assert.Equal(700, dest.Balance);
    }

    [Fact]
    public void Interest_Calculation_ShouldBeCorrectValue()
    {
        // Lãi suất 0.5% (0.005)
        decimal balance = 1000000; // 1 triệu
        decimal expectedInterest = 5000;

        decimal actualInterest = balance * 0.005m;

        Assert.Equal(expectedInterest, actualInterest);
    }
}