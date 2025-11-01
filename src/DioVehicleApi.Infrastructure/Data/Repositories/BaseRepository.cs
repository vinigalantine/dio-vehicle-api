using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DioVehicleApi.Infrastructure.Data.Repositories;

public abstract class BaseRepository<TEntity, TKey> : IBaseRepository<TEntity, TKey>
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
        var query = ApplyFilters(DbSet);
        return await query.ToListAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        int? skip = null,
        int? take = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = DbSet;

        query = ApplyFilters(query);

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (skip.HasValue)
        {
            query = query.Skip(skip.Value);
        }

        if (take.HasValue)
        {
            query = query.Take(take.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = DbSet;

        query = ApplyFilters(query);

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.CountAsync(cancellationToken);
    }

    protected virtual IQueryable<TEntity> ApplyFilters(IQueryable<TEntity> query)
    {
        return query;
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

    public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await Context.SaveChangesAsync(cancellationToken);
    }
}
