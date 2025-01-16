using System.Linq.Expressions;
using BL.Interfaces;
using DAL.Interfaces;

namespace BL.Implementations;

/// <summary>
/// The Manager class that implements the IManager interface.
/// </summary>
/// <typeparam name="TEntity">The type of the entity to be managed. This must be a class.</typeparam>
public class Manager<TEntity>(IRepository repository) : IManager<TEntity>
    where TEntity : class
{
    public async Task<IEnumerable<TEntity>> GetAllAsync() => await repository.GetAllAsync<TEntity>();

    public async Task<TEntity> GetAsync(int id) => await repository.GetAsync<TEntity>(id);

    public async Task AddAsync(TEntity entity) => await repository.AddAsync(entity);

    public async Task UpdateAsync(TEntity entityToUpdate, TEntity entity) =>
        await repository.UpdateAsync(entityToUpdate, entity);
    
    public async Task UpdateAllAsync(IEnumerable<TEntity> entitiesToUpdate)
    {
        await repository.UpdateAllAsync(entitiesToUpdate);
    }
    public async Task DeleteAsync(TEntity entity) => await repository.DeleteAsync(entity);
    public Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return repository.FindAsync(predicate);
    }
    
}