using DioVehicleApi.Domain.Entities;
using MediatR;

namespace DioVehicleApi.Application.Features.Vehicles.Commands.UpdateVehicle;

/// <summary>
/// Command to update a vehicle
/// </summary>
public record UpdateVehicleCommand : IRequest<Vehicle>
{
    public required Guid Id { get; init; }
    public required int Year { get; init; }
    public required string Color { get; init; }
    public required string LicensePlate { get; init; }
    public required Guid ModelId { get; init; }
}
