using System.Linq.Expressions;
using DioVehicleApi.Application.Contracts.Base;
using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Interfaces.Repositories;
using MediatR;

namespace DioVehicleApi.Application.Features.Vehicles.Queries.GetAllVehicles;

/// <summary>
/// Handler for retrieving all vehicles
/// </summary>
public class GetAllVehiclesQueryHandler : IRequestHandler<GetAllVehiclesQuery, PaginatedResult<Vehicle>>
{
    private readonly IVehicleRepository _vehicleRepository;

    public GetAllVehiclesQueryHandler(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public async Task<PaginatedResult<Vehicle>> Handle(GetAllVehiclesQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Vehicle, bool>> filter = vehicle => 
            (!request.Year.HasValue || vehicle.Year == request.Year.Value) &&
            (string.IsNullOrEmpty(request.Color) || vehicle.Color.Contains(request.Color)) &&
            (string.IsNullOrEmpty(request.LicensePlate) || vehicle.LicensePlate.Contains(request.LicensePlate));

        var vehicles = await _vehicleRepository.GetAllAsync(
            filter: filter,
            skip: (request.PageNumber - 1) * request.PageSize, 
            take: request.PageSize, 
            cancellationToken: cancellationToken
        );

        var totalCount = await _vehicleRepository.CountAsync(
            filter: filter,
            cancellationToken: cancellationToken
        );

        return new PaginatedResult<Vehicle>
        {
            Items = vehicles,
            TotalCount = totalCount
        };
    }
}

