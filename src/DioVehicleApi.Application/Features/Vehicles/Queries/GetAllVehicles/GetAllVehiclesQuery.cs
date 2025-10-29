using DioVehicleApi.Application.Contracts.Base;
using DioVehicleApi.Application.Features.BaseFeature.Queries;
using DioVehicleApi.Domain.Entities;
using MediatR;

namespace DioVehicleApi.Application.Features.Vehicles.Queries.GetAllVehicles;

/// <summary>
/// Query to retrieve all vehicles
/// </summary>
public record GetAllVehiclesQuery : Pagination, IRequest<PaginatedResult<Vehicle>>
{
    public int? Year { get; set; }
    public string? Color { get; set; }
    public string? LicensePlate { get; set; }
}
