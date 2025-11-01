using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Exceptions;
using DioVehicleApi.Domain.Interfaces.Repositories;
using MediatR;

namespace DioVehicleApi.Application.Features.Vehicles.Commands.UpdateVehicle;

/// <summary>
/// Handler for updating a vehicle
/// </summary>
public class UpdateVehicleCommandHandler : IRequestHandler<UpdateVehicleCommand, Vehicle>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IModelRepository _modelRepository;

    public UpdateVehicleCommandHandler(IVehicleRepository vehicleRepository, IModelRepository modelRepository)
    {
        _vehicleRepository = vehicleRepository;
        _modelRepository = modelRepository;
    }

    public async Task<Vehicle> Handle(UpdateVehicleCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(request.Id, cancellationToken);

        if (vehicle == null) throw new NotFoundException(nameof(Vehicle), request.Id);
        
        if (vehicle.ModelId != request.ModelId) await ValidateIfModelExistsAndActiveAsync(request.ModelId, cancellationToken);

        vehicle.Year = request.Year;
        vehicle.Color = request.Color;
        vehicle.LicensePlate = request.LicensePlate;
        vehicle.ModelId = request.ModelId;

        var updated = _vehicleRepository.Update(vehicle);
        await _vehicleRepository.SaveChangesAsync(cancellationToken);

        return updated;
    }

    private async Task ValidateIfModelExistsAndActiveAsync(Guid modelId, CancellationToken cancellationToken)
    {
        var model = await _modelRepository.GetByIdAsync(modelId, cancellationToken);

        if (model == null) throw new NotFoundException(nameof(Model), modelId);

        if (model.IsDeleted) throw new DomainException($"Cannot create vehicle for inactive model '{model.Name}'");
    }
}
