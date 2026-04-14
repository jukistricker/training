using BankAccountSimulation.Domain.Entities;
using BankAccountSimulation.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankAccountSimulation.Infra.Persistence;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        try
        { 
            if(await context.BankAccounts.AnyAsync())
            {
                return;
            }

            var random = new Random();
            var accounts = new List<BankAccount>();
            var transactions = new List<Transaction>();

            logger.LogInformation("Bắt đầu khởi tạo dữ liệu bằng vòng lặp...");

            for (int i = 1; i <= 100; i++)
            {
                var account = new BankAccount
                {
                    AccountNumber = $"ACC{1000 + i}",
                    OwnerName = (i == 1) ? "System Admin" : $"User Number {i}",
                    Role = (i == 1) ? "Admin" : "User", 
                    Balance = random.Next(1000, 50000),
                    PasswordHash = "240BE518FABD2724DDB6F04EEB1DA5967448D7E831C08C8FA822809F74C720A9", // admin123
                    Status = AccountStatus.Active,
                    CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 365))
                };
                accounts.Add(account);
            }

            await context.BankAccounts.AddRangeAsync(accounts);
            await context.SaveChangesAsync();

            for (int j = 1; j <= 300; j++)
            {
                var randomAccount = accounts[random.Next(accounts.Count)];
                var type = (TransactionType)random.Next(0, 2);
                var transaction = new Transaction
                {
                    AccountNumber = randomAccount.AccountNumber,
                    Amount = random.Next(10, 500),
                    Type = (TransactionType)random.Next(0, 2),
                    Description = type == TransactionType.Deposit ? $"Nạp tiền vào tài khoản {j}" : $"Rút tiền từ tài khoản {j}",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-random.Next(1, 10000))
                };
                transactions.Add(transaction);
            }

            await context.Transactions.AddRangeAsync(transactions);
            await context.SaveChangesAsync();

            logger.LogInformation($"Thành công: Đã tạo {accounts.Count} accounts và {transactions.Count} transactions.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Lỗi xảy ra khi chạy vòng lặp Seed dữ liệu.");
        }
    }
}
