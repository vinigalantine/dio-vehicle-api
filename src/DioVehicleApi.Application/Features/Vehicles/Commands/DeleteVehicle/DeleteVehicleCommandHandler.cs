using DioVehicleApi.Domain.Interfaces.Repositories;
using MediatR;

namespace DioVehicleApi.Application.Features.Vehicles.Commands.DeleteVehicle;

/// <summary>
/// Handler for deleting a vehicle
/// </summary>
public class DeleteVehicleCommandHandler : IRequestHandler<DeleteVehicleCommand, bool>
{
    private readonly IVehicleRepository _vehicleRepository;

    public DeleteVehicleCommandHandler(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public async Task<bool> Handle(DeleteVehicleCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (vehicle == null)
        {
            return false;
        }

        _vehicleRepository.Remove(vehicle);
        var rowsAffected = await _vehicleRepository.SaveChangesAsync(cancellationToken);
        
        return rowsAffected > 0;
    }
}
