using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Interfaces.Repositories;
using MediatR;

namespace DioVehicleApi.Application.Features.Vehicles.Queries.GetVehicle;

public class GetVehicleQueryHandler : IRequestHandler<GetVehicleQuery, Vehicle?>
{
    private readonly IVehicleRepository _vehicleRepository;

    public GetVehicleQueryHandler(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public async Task<Vehicle?> Handle(GetVehicleQuery request, CancellationToken cancellationToken)
    {

        return await _vehicleRepository.GetByIdAsync(request.Id, cancellationToken);
    }
}
