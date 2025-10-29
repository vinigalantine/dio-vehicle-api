using DioVehicleApi.Domain.Entities;

namespace DioVehicleApi.Domain.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<User, Guid>
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
}
