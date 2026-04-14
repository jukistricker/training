
using BankAccountSimulation.Domain.Entities;
using BankAccountSimulation.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace BankAccountSimulation.Infra.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    private IDbContextTransaction _currentTransaction;

    public IRepository<BankAccount> Accounts { get; }
    public IRepository<Transaction> Transactions { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Accounts = new Repository<BankAccount>(_context);
        Transactions = new Repository<Transaction>(_context);
    }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }


    public async Task BeginTransactionAsync()
    {
        _currentTransaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            await _currentTransaction.CommitAsync();
        }
        finally
        {
            _currentTransaction.Dispose();
        }
    }

    public async Task RollbackAsync()
    {
        await _currentTransaction.RollbackAsync();
        _currentTransaction.Dispose();
    }
}