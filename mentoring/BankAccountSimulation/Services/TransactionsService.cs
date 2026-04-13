using BankAccountSimulation.Data;
using BankAccountSimulation.Models;
using BankAccountSimulation.Services.Interfaces;
using BankAccountSimulation.ViewModels;

namespace BankAccountSimulation.Services;

public class TransactionsService : ITransactionsService
{
    private readonly JsonDbContext _context;

    public TransactionsService(JsonDbContext context)
    {
        _context = context;
    }

    public async Task<OperationResult> ProcessTransaction(string accountNumber, decimal amount, TransactionType type, string description)
    {
        var account = _context.Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);

        if (account == null) return OperationResult.Fail("Account does not exist.");
        if (account.Status == AccountStatus.Frozen) return OperationResult.Fail("Account is frozen, can not operate this action.");
        if (amount <= 0) return OperationResult.Fail("Amount must be greater than 0.");

        if (type == TransactionType.Withdraw || type == TransactionType.Transfer)
        {
            if (account.Balance - amount < 100)
                return OperationResult.Fail("Current balance is not enough, minimum balance is 100.");
        }

        if (type == TransactionType.Deposit) account.Balance += amount;
        else account.Balance -= amount;

        var transaction = new Transaction
        {
            Id = _context.Transactions.Count + 1,
            AccountNumber = accountNumber,
            Type = type,
            Amount = amount,
            CreatedAt = DateTime.Now,
            Description = description
        };

        _context.Transactions.Add(transaction);
        _context.SaveAccounts();
        _context.SaveTransactions();

        return OperationResult.Success();
    }

    public async Task<OperationResult> Transfer(string sourceId, string destId, decimal amount, string? description)
    {
        var sourceAcc = _context.Accounts.FirstOrDefault(a => a.AccountNumber == sourceId);
        var destAcc = _context.Accounts.FirstOrDefault(a => a.AccountNumber == destId);

        if (sourceAcc == null) return OperationResult.Fail("Source account does not exist.");
        if (destAcc == null) return OperationResult.Fail("Destination account does not exist.");
        if (sourceId == destId) return OperationResult.Fail("Can not transfer money to yourself.");

        var result = await ProcessTransaction(sourceId, amount, TransactionType.Transfer, description ?? $"Transfer money to: {destId}");

        if (!result.IsSuccess) return result;

        destAcc.Balance += amount;
        _context.Transactions.Add(new Transaction
        {
            Id = _context.Transactions.Count + 1,
            AccountNumber = destId,
            Type = TransactionType.Transfer,
            Amount = amount,
            CreatedAt = DateTime.Now,
            Description = $"Receive money from: {sourceId}"
        });

        _context.SaveAccounts();
        _context.SaveTransactions();

        return OperationResult.Success();
    }

    public async Task<(IEnumerable<Transaction> Items, int TotalCount)> GetHistory(string accountNumber, string type, int page)
    {
        var query = _context.Transactions.AsQueryable();

        if (accountNumber != "ADMIN001")
        {
            query = query.Where(t => t.AccountNumber == accountNumber);
        }

        switch (type?.ToLower())
        {
            case "deposit":
                query = query.Where(t => t.Type == TransactionType.Deposit);
                break;
            case "withdraw":
                query = query.Where(t => t.Type == TransactionType.Withdraw);
                break;
            case "transfer":
                query = query.Where(t => t.Type == TransactionType.Transfer);
                break;
            case "all":
            default:
                break;
        }

        int totalCount = query.Count();
        int pageSize = 10;

        var items = query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return await Task.FromResult((items, totalCount));
    }
}