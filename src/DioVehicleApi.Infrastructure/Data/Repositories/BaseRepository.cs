using DioVehicleApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DioVehicleApi.Infrastructure.Data.Repositories;

public abstract class BaseRepository<TEntity, TKey> 
    where TEntity : BaseEntity<TKey>
    where TKey : IEquatable<TKey>
{
    protected readonly DbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    protected BaseRepository(DbContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        DbSet = Context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync([id!], cancellationToken);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllIncludingDeletedAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.IgnoreQueryFilters().ToListAsync(cancellationToken);
    }

    public virtual async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        return (await DbSet.AddAsync(entity, cancellationToken)).Entity;
    }

    public virtual TEntity Update(TEntity entity)
    {
        return DbSet.Update(entity).Entity;
    }

    public virtual void Remove(TEntity entity)
    {
        DbSet.Remove(entity);
    }
}
