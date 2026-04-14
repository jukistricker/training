using Microsoft.EntityFrameworkCore;
using BankAccountSimulation.Domain.Entities;
using BankAccountSimulation.Domain.Enums;
using BankAccountSimulation.Domain.Interfaces;
using BankAccountSimulation.Models;
using BankAccountSimulation.Services.Interfaces;

namespace BankAccountSimulation.Services;

public class TransactionsService : ITransactionsService
{
    private readonly IUnitOfWork _unitOfWork;

    public TransactionsService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<OperationResult> ProcessTransaction(string accountNumber, decimal amount, TransactionType type, string description)
    {
        if (amount <= 0) return OperationResult.Fail("Amount must be greater than 0.");

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var account = await _unitOfWork.Accounts.FindAsync(a => a.AccountNumber == accountNumber);

            if (account == null)
            {
                await _unitOfWork.RollbackAsync(); 
                return OperationResult.Fail("Account does not exist.");
            }

            if (account.Status == AccountStatus.Frozen)
            {
                await _unitOfWork.RollbackAsync();
                return OperationResult.Fail("Account is frozen.");
            }

            if (type == TransactionType.Withdraw || type == TransactionType.Transfer)
            {
                if (account.Balance - amount < 100)
                {
                    await _unitOfWork.RollbackAsync();
                    return OperationResult.Fail("Current balance is not enough, minimum balance is 100.");
                }
            }

            if (type == TransactionType.Deposit)
            {
                account.Balance += amount;
            }
            else
            {
                account.Balance -= amount;
            }

            var transaction = new Transaction
            {
                AccountNumber = accountNumber,
                Type = type,
                Amount = amount,
                CreatedAt = DateTime.UtcNow, 
                Description = description
            };

            await _unitOfWork.Transactions.AddAsync(transaction);
            _unitOfWork.Accounts.Update(account);

            await _unitOfWork.CommitAsync();

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return OperationResult.Fail("Transaction failed: " + ex.Message);
        }
    }

    public async Task<OperationResult> Transfer(string srcAccount, string destAccount, decimal amount, string? description)
    {
        if (amount <= 0) return OperationResult.Fail("Amount must be greater than 0.");
        if (srcAccount == destAccount) return OperationResult.Fail("Can not transfer money to yourself.");

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var source = await _unitOfWork.Accounts.FindAsync(a => a.AccountNumber == srcAccount);
            var dest = await _unitOfWork.Accounts.FindAsync(a => a.AccountNumber == destAccount);

            if (source == null)
            {
                await _unitOfWork.RollbackAsync();
                return OperationResult.Fail("Source account does not exist.");
            }
            if (dest == null)
            {
                await _unitOfWork.RollbackAsync();
                return OperationResult.Fail("Destination account does not exist.");
            }
            if (source.Balance - amount < 100)
            {
                await _unitOfWork.RollbackAsync();
                return OperationResult.Fail("Source balance is not enough (min 100).");
            }

            source.Balance -= amount;
            dest.Balance += amount;

            var srcLog = new Transaction
            {
                AccountNumber = srcAccount,
                Type = TransactionType.Transfer,
                Amount = amount,
                CreatedAt = DateTime.UtcNow,
                Description = description ?? $"Transfer to {destAccount}"
            };

            var destLog = new Transaction
            {
                AccountNumber = destAccount,
                Type = TransactionType.Transfer, 
                Amount = amount,
                CreatedAt = DateTime.UtcNow,
                Description = description ?? $"Receive from {srcAccount}"
            };

            await _unitOfWork.Transactions.AddAsync(srcLog);
            await _unitOfWork.Transactions.AddAsync(destLog);

            _unitOfWork.Accounts.Update(source);
            _unitOfWork.Accounts.Update(dest);

            await _unitOfWork.CommitAsync();

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return OperationResult.Fail("Transfer failed: " + ex.Message);
        }
    }

    public async Task<(IEnumerable<Transaction> Items, int TotalCount)> GetHistory(string accountNumber, string? type, int page)
    {
        var account = await _unitOfWork.Accounts.FindAsync(a => a.AccountNumber == accountNumber);
        if (account == null) return (Enumerable.Empty<Transaction>(), 0);

        // 1. Lấy Queryable thay vì GetAll (Dữ liệu vẫn nằm ở DB, chưa tải về RAM)
        var query = _unitOfWork.Transactions.GetQueryable();

        // 2. Build câu lệnh SQL (Chỉ là cộng dồn chuỗi lệnh, chưa chạy)
        if (account.Role != "Admin")
        {
            query = query.Where(t => t.AccountNumber == accountNumber);
        }

        if (!string.IsNullOrEmpty(type) && Enum.TryParse<TransactionType>(type, true, out var transactionType))
        {
            query = query.Where(t => t.Type == transactionType);
        }

        // 3. Thực thi COUNT trên DB (SQL: SELECT COUNT(*) FROM ...)
        int totalCount = await query.CountAsync();

        // 4. Thực thi PHÂN TRANG trên DB (SQL: SELECT ... OFFSET x LIMIT y)
        int pageSize = 10;
        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}