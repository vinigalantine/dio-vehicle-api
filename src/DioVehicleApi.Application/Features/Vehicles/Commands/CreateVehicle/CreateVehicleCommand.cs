using DioVehicleApi.Domain.Entities;
using MediatR;

namespace DioVehicleApi.Application.Features.Vehicles.Commands.CreateVehicle;

public record CreateVehicleCommand : IRequest<Vehicle>
{
    public required int Year { get; init; }
    public required string Color { get; init; }
    public required string LicensePlate { get; init; }
    public required Guid ModelId { get; init; }
}
