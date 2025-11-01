using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DioVehicleApi.Infrastructure.Data.Repositories;

public class BrandRepository : BaseRepository<Brand, Guid>, IBrandRepository
{
    public BrandRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Brand>()
            .AnyAsync(b => b.Name == name, cancellationToken);
    }

    protected override IQueryable<Brand> ApplyFilters(IQueryable<Brand> query)
    {
        return query
            .OrderBy(b => b.Name);
    }
}
