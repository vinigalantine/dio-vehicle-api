using DioVehicleApi.Domain.ValueObjects;

namespace DioVehicleApi.Domain.Services;

public interface IAuthService
{
    Task<Token?> AuthenticateAsync(string username, string password);
    Task<bool> ValidateTokenAsync(string token);
}
