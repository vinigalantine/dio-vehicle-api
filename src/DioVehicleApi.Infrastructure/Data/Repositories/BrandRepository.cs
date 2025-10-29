using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Interfaces.Repositories;

namespace DioVehicleApi.Infrastructure.Data.Repositories;

public class BrandRepository : BaseRepository<Brand, Guid>, IBrandRepository
{
    public BrandRepository(ApplicationDbContext context) : base(context)
    {
    }

    protected override IQueryable<Brand> ApplyFilters(IQueryable<Brand> query)
    {
        return query
            .OrderBy(b => b.Name);
    }
}
