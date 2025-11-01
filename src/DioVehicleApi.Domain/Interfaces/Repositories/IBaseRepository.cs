using DioVehicleApi.Domain.Entities;
using System.Linq.Expressions;

namespace DioVehicleApi.Domain.Interfaces.Repositories;

/// <summary>
/// Base repository interface - defines the contract for data access
/// Lives in Domain layer (no infrastructure dependencies)
/// </summary>
public interface IBaseRepository<TEntity, TKey>
    where TEntity : BaseEntity<TKey>
    where TKey : IEquatable<TKey>
{
    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null, int? skip = null, int? take = null, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllIncludingDeletedAsync(CancellationToken cancellationToken = default);
    Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
    TEntity Update(TEntity entity);
    void Remove(TEntity entity);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
