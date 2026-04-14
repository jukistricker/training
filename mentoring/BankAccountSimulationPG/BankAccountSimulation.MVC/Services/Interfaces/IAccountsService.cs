using BankAccountSimulation.Domain.Enums;
using BankAccountSimulation.Models;
using BankAccountSimulation.ViewModels;

namespace BankAccountSimulation.Services.Interfaces;

public interface IAccountsService
{
    Task<IEnumerable<AccountsViewModel>> GetAllAccounts();
    Task<AccountsViewModel> GetAccountDetails(string accountNumber);
    Task<OperationResult> CreateAccount(CreateAccountViewModel request);
    Task<OperationResult> Login(LoginAccountViewModel request);
    Task<OperationResult> UpdateAccountStatus(string accountNumber, AccountStatus status);
}