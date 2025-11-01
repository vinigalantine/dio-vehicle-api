using MediatR;

namespace DioVehicleApi.Application.Features.Auth.Commands.Login;

/// <summary>
/// Command to authenticate a user
/// </summary>
public record LoginCommand : IRequest<LoginResponse?>
{
    public required string Username { get; init; }
    public required string Password { get; init; }
}
