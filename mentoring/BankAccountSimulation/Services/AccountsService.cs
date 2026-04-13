using BankAccountSimulation.Data;
using BankAccountSimulation.Models;
using BankAccountSimulation.Services.Interfaces;
using BankAccountSimulation.ViewModels;
using System.Security.Cryptography;
using System.Text;

namespace BankAccountSimulation.Services;

public class AccountsService : IAccountsService
{
    private readonly JsonDbContext _context;

    public AccountsService(JsonDbContext context)
    {
        _context = context;
    }

    public Task<IEnumerable<AccountsViewModel>> GetAllAccounts()
    {
        var result = _context.Accounts.Select(a => new AccountsViewModel
        {
            AccountNumber = a.AccountNumber,
            OwnerName = a.OwnerName,
            Role = a.Role,
            Balance = a.Balance,
            Status = a.Status.ToString(),
            CreatedAt = a.CreatedAt
        }).ToList();
        return Task.FromResult<IEnumerable<AccountsViewModel>>(result);
    }

    public async Task<AccountsViewModel?> GetAccountDetails(string accountNumber)
    {
        var account = _context.Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);

        if (account == null) return null; 

        return new AccountsViewModel
        {
            AccountNumber = account.AccountNumber,
            OwnerName = account.OwnerName,
            Role = account.Role,
            Balance = account.Balance,
            Status = account.Status.ToString(),
            CreatedAt = account.CreatedAt
        };
    }
    public async Task<OperationResult> CreateAccount(CreateAccountViewModel request)
    {
        if (_context.Accounts.Any(a => a.AccountNumber == request.AccountNumber))
        {
            return OperationResult.Fail("Account number already exists", nameof(request.AccountNumber));
        }

        var newAccount = new BankAccount
        {
            AccountNumber = request.AccountNumber,
            OwnerName = request.OwnerName,
            Balance = request.InitialBalance,
            PasswordHash = _context.HashPassword(request.Password),
            Role = "User",
            Status = AccountStatus.Active,
            CreatedAt = DateTime.Now
        };

        _context.Accounts.Add(newAccount);
        _context.SaveAccounts(); 

        return OperationResult.Success();
    }

    public async Task<OperationResult> Login(LoginAccountViewModel request)
    {
        var account = _context.Accounts.FirstOrDefault(a => a.AccountNumber == request.AccountNumber);

        if (account == null)
            return OperationResult.Fail("Account number does not exist", nameof(request.AccountNumber));

        if (account.PasswordHash != _context.HashPassword(request.Password))
            return OperationResult.Fail("Invalid password", nameof(request.Password));

        return OperationResult.Success();
    }

    public async Task<OperationResult> UpdateAccountStatus(string accountNumber, AccountStatus status)
    {
        var account = _context.Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
        if (account == null)
        {
            return OperationResult.Fail("Bank account not found.");
        }

        account.Status = status;
        _context.SaveAccounts();

        return OperationResult.Success();
    }

    
}