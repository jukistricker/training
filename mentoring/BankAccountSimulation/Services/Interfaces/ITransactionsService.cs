using BankAccountSimulation.Models;
using BankAccountSimulation.ViewModels;

namespace BankAccountSimulation.Services.Interfaces
{
    public interface ITransactionsService
    {
        Task<OperationResult> ProcessTransaction(string accountNumber, decimal amount, TransactionType type, string description);

        Task<OperationResult> Transfer(string sourceAccountNumber, string destinationAccountNumber, decimal amount, string? description);

        Task<(IEnumerable<Transaction> Items, int TotalCount)> GetHistory(string accountNumber, string type, int page);
    }
}