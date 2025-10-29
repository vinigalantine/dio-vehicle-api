using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Linq.Expressions;
using System.Reflection;

namespace DioVehicleApi.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor)
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<Brand> Brands { get; set; }
    public DbSet<Model> Models { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        ApplySoftDeleteQueryFilters(modelBuilder);
    }

    public override int SaveChanges()
    {
        UpdateAuditFields();
        HandleSoftDeletes();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        HandleSoftDeletes();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplySoftDeleteQueryFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
                var filter = Expression.Lambda(Expression.Not(property), parameter);
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }
    }

    private void UpdateAuditFields()
    {
        var currentUser = GetCurrentUserFromJWT();
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity.GetType().IsGenericType && 
                       e.Entity.GetType().GetGenericTypeDefinition() == typeof(BaseEntity<>).GetGenericTypeDefinition() ||
                       e.Entity.GetType().BaseType?.IsGenericType == true &&
                       e.Entity.GetType().BaseType?.GetGenericTypeDefinition() == typeof(BaseEntity<>));

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    SetPropertyValue(entry.Entity, nameof(BaseEntity<object>.CreatedAt), DateTime.UtcNow);
                    SetPropertyValue(entry.Entity, nameof(BaseEntity<object>.CreatedBy), currentUser);
                    break;
                case EntityState.Modified:
                    SetPropertyValue(entry.Entity, nameof(BaseEntity<object>.UpdatedAt), DateTime.UtcNow);
                    SetPropertyValue(entry.Entity, nameof(BaseEntity<object>.UpdatedBy), currentUser);
                    break;
            }
        }
    }

    private void HandleSoftDeletes()
    {
        var currentUser = GetCurrentUserFromJWT();
        var softDeleteEntries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Deleted && e.Entity is ISoftDeletable);

        foreach (var entry in softDeleteEntries)
        {
            entry.State = EntityState.Modified;
            var softDeletable = (ISoftDeletable)entry.Entity;
            softDeletable.IsDeleted = true;
            softDeletable.DeletedAt = DateTime.UtcNow;
            softDeletable.DeletedBy = currentUser;
        }
    }

    private string GetCurrentUserFromJWT()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context?.User?.Identity?.IsAuthenticated == true)
        {
            return context.User.FindFirst(ClaimTypes.Name)?.Value 
                ?? context.User.FindFirst("username")?.Value
                ?? context.User.FindFirst(ClaimTypes.Email)?.Value
                ?? context.User.FindFirst("sub")?.Value
                ?? "Unknown";
        }
        
        return "System";
    }

    private static void SetPropertyValue(object entity, string propertyName, object value)
    {
        var property = entity.GetType().GetProperty(propertyName);
        if (property != null && property.CanWrite)
        {
            property.SetValue(entity, value);
        }
    }
}