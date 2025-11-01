using DioVehicleApi.Domain.Entities;

namespace DioVehicleApi.Domain.Interfaces.Repositories;

public interface IBrandRepository : IBaseRepository<Brand, Guid>
{
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
}
