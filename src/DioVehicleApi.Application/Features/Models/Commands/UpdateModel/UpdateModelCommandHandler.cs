using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Exceptions;
using DioVehicleApi.Domain.Interfaces.Repositories;
using MediatR;

namespace DioVehicleApi.Application.Features.Models.Commands.UpdateModel;

/// <summary>
/// Handler for updating a model
/// </summary>
public class UpdateModelCommandHandler : IRequestHandler<UpdateModelCommand, Model>
{
    private readonly IModelRepository _modelRepository;
    private readonly IBrandRepository _brandRepository;

    public UpdateModelCommandHandler(IModelRepository modelRepository, IBrandRepository brandRepository)
    {
        _modelRepository = modelRepository;
        _brandRepository = brandRepository;
    }

    public async Task<Model> Handle(UpdateModelCommand request, CancellationToken cancellationToken)
    {
        var model = await _modelRepository.GetByIdAsync(request.Id, cancellationToken);

        if (model == null) throw new NotFoundException(nameof(Model), request.Id);

        if (model.BrandId != request.BrandId) await ValidateIfBrandExistsAndActiveAsync(request.BrandId, cancellationToken);

        model.Name = request.Name;
        model.BrandId = request.BrandId;

        var updated = _modelRepository.Update(model);
        await _modelRepository.SaveChangesAsync(cancellationToken);

        return updated;
    }

    private async Task ValidateIfBrandExistsAndActiveAsync(Guid brandId, CancellationToken cancellationToken)
    {
        var brand = await _brandRepository.GetByIdAsync(brandId, cancellationToken);

        if (brand == null) throw new NotFoundException(nameof(Brand), brandId);
        
        if (brand.IsDeleted) throw new DomainException($"Cannot assign model to inactive brand '{brand.Name}'");
    }
}
