using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DioVehicleApi.Infrastructure.Data.Repositories;

public class VehicleRepository : BaseRepository<Vehicle, Guid>, IVehicleRepository
{
    public VehicleRepository(ApplicationDbContext context) : base(context)
    {
    }

    protected override IQueryable<Vehicle> ApplyFilters(IQueryable<Vehicle> query)
    {
        return query
            .OrderBy(b => b.CreatedAt)
            .Include(b => b.Model);
    }
}
