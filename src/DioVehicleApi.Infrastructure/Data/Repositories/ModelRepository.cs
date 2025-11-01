using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DioVehicleApi.Infrastructure.Data.Repositories;

public class ModelRepository : BaseRepository<Model, Guid>, IModelRepository
{
    public ModelRepository(ApplicationDbContext context) : base(context)
    {
    }

    protected override IQueryable<Model> ApplyFilters(IQueryable<Model> query)
    {
        return query
            .OrderBy(b => b.Name)
            .Include(b => b.Brand);
    }
}
