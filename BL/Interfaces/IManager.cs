using System.Linq.Expressions;

namespace BL.Interfaces;

//TODO: Be sure that the IManager is set up correctly for the project.
/// <summary>
/// Defines a generic interface to communicate with the DB repository
/// </summary>
/// <typeparam name="TEntity">The type of the entity to be managed. This must be a class</typeparam>
public interface IManager<TEntity> where TEntity : class
{
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity> GetAsync(int id);
    Task AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entityToUpdate, TEntity entity);
    Task UpdateAllAsync(IEnumerable<TEntity> entitiesToUpdate);
    Task DeleteAsync(TEntity entity);
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
}