using BankAccountSimulation.Domain.Common;
using BankAccountSimulation.Domain.Entities;
using BankAccountSimulation.Domain.Enums;
using BankAccountSimulation.Domain.Interfaces;
using BankAccountSimulation.Models;
using BankAccountSimulation.Services.Interfaces;
using BankAccountSimulation.ViewModels;
using System.Security.Cryptography;

namespace BankAccountSimulation.Services;

public class AccountsService : IAccountsService
{
    private readonly IUnitOfWork _unitOfWork;

    public AccountsService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<AccountsViewModel>> GetAllAccounts()
    {
        var accounts = await _unitOfWork.Accounts.GetAllAsync();

        return accounts.Select(a => new AccountsViewModel
        {
            AccountNumber = a.AccountNumber,
            OwnerName = a.OwnerName,
            Role = a.Role,
            Balance = a.Balance,
            Status = a.Status.ToString(),
            CreatedAt = a.CreatedAt
        });
    }

    public async Task<AccountsViewModel?> GetAccountDetails(string accountNumber)
    {
        var account =await _unitOfWork.Accounts.FindAsync(a=>a.AccountNumber==accountNumber);

        if (account == null)
        {
            return null;
        }

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
        if (await _unitOfWork.Accounts.FindAsync(a => a.AccountNumber == request.AccountNumber) != null)
        {
            return OperationResult.Fail("Account number already exists", nameof(request.AccountNumber));
        }

        var newAccount = new BankAccount
        {
            AccountNumber = request.AccountNumber,
            OwnerName = request.OwnerName,
            Balance = request.InitialBalance,
            PasswordHash = SecurityHelper.HashData<SHA256>(request.Password),
            Role = "User",
            Status = AccountStatus.Active,
            CreatedAt = DateTime.Now
        };

        await _unitOfWork.Accounts.AddAsync(newAccount);
        await _unitOfWork.CompleteAsync(); 

        return OperationResult.Success();
    }

    public async Task<OperationResult> Login(LoginAccountViewModel request)
    {
        var account =await _unitOfWork.Accounts.FindAsync(a => a.AccountNumber == request.AccountNumber);

        if (account == null)
            return OperationResult.Fail("Account number does not exist", nameof(request.AccountNumber));

        if (account.PasswordHash != SecurityHelper.HashData<SHA256>(request.Password))
            return OperationResult.Fail("Invalid password", nameof(request.Password));

        return OperationResult.Success();
    }

    public async Task<OperationResult> UpdateAccountStatus(string accountNumber, AccountStatus status)
    {
        var account = await _unitOfWork.Accounts.FindAsync(a => a.AccountNumber == accountNumber);
        if (account == null)
        {
            return OperationResult.Fail("Bank account not found.");
        }

        account.Status = status;
        await _unitOfWork.CompleteAsync();

        return OperationResult.Success();
    }

    
}