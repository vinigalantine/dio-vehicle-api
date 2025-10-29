using MediatR;

namespace DioVehicleApi.Application.Features.Vehicles.Commands.DeleteVehicle;

/// <summary>
/// Command to delete a vehicle
/// </summary>
public record DeleteVehicleCommand : IRequest<bool>
{
    public required Guid Id { get; init; }
}
