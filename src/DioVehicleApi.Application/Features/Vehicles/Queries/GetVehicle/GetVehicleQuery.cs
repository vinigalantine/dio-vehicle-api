using DioVehicleApi.Domain.Entities;
using MediatR;

namespace DioVehicleApi.Application.Features.Vehicles.Queries.GetVehicle;

/// <summary>
/// Query to retrieve a vehicle by ID
/// </summary>
public record GetVehicleQuery : IRequest<Vehicle?>
{
    public required Guid Id { get; init; }
}
