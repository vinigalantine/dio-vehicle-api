using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Exceptions;
using DioVehicleApi.Domain.Interfaces.Repositories;
using MediatR;

namespace DioVehicleApi.Application.Features.Vehicles.Commands.CreateVehicle;

/// <summary>
/// Handler for creating a vehicle
/// </summary>
public class CreateVehicleCommandHandler : IRequestHandler<CreateVehicleCommand, Vehicle>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IModelRepository _modelRepository;

    public CreateVehicleCommandHandler(IVehicleRepository vehicleRepository, IModelRepository modelRepository)
    {
        _vehicleRepository = vehicleRepository;
        _modelRepository = modelRepository;
    }

    public async Task<Vehicle> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        await ValidateIfModelExistsAndActiveAsync(request.ModelId, cancellationToken);

        var vehicle = new Vehicle
        {
            Id = Guid.NewGuid(),
            Year = request.Year,
            Color = request.Color,
            LicensePlate = request.LicensePlate,
            ModelId = request.ModelId
        };

        await _vehicleRepository.CreateAsync(vehicle, cancellationToken);
        await _vehicleRepository.SaveChangesAsync(cancellationToken);

        return vehicle;
    }

    private async Task ValidateIfModelExistsAndActiveAsync(Guid modelId, CancellationToken cancellationToken)
    {
        var model = await _modelRepository.GetByIdAsync(modelId, cancellationToken);

        if (model == null) throw new NotFoundException(nameof(Model), modelId);

        if (model.IsDeleted) throw new DomainException($"Cannot create vehicle for inactive model '{model.Name}'");
    }
}
