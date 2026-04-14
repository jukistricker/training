using BankAccountSimulation.Domain.Entities;

namespace BankAccountSimulation.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    public IRepository<BankAccount> Accounts { get; }
    public IRepository<Transaction> Transactions { get; }
    Task<int> CompleteAsync();
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}