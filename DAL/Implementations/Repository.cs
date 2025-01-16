using System.Linq.Expressions;
using DAL.EF;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Implementations;

/// <summary>
/// The Repository class that implements the IRepository interface.
/// </summary>
public class Repository(PhygitalDbContext context) : IRepository
{
    public async Task<IEnumerable<TEntity>> GetAllAsync<TEntity>() where TEntity : class =>
        await context.Set<TEntity>().ToListAsync();

    public async Task<TEntity> GetAsync<TEntity>(int id) where TEntity : class =>
        await context.Set<TEntity>().FindAsync(id);

    public async Task AddAsync<TEntity>(TEntity entity) where TEntity : class
    {
        context.Set<TEntity>().Add(entity);
    }

    public async Task UpdateAsync<TEntity>(TEntity entityToUpdate, TEntity entity) where TEntity : class
    {
        context.Entry(entityToUpdate).CurrentValues.SetValues(entity);
    }

    public async Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class
    {
        context.Set<TEntity>().Remove(entity);
    }

    public Task<IEnumerable<TEntity>> FindAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
        where TEntity : class
    {
        return Task.FromResult(context.Set<TEntity>().Where(predicate).AsEnumerable());
    }

    public async Task UpdateAllAsync<TEntity>(IEnumerable<TEntity> entitiesToUpdate) where TEntity : class
    {
        context.UpdateRange(entitiesToUpdate);
    }
}