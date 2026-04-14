using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace BankAccountSimulation.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    IQueryable<T> GetQueryable();
    Task<T?> FindAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}
