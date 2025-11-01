using DioVehicleApi.Domain.ValueObjects;

namespace DioVehicleApi.Application.Features.Auth.Commands.Login;

/// <summary>
/// Response for login command
/// </summary>
public record LoginResponse
{
    public required string Token { get; init; }
    public required DateTime ExpiresAt { get; init; }
}
