using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Exceptions;
using DioVehicleApi.Domain.Interfaces.Repositories;
using MediatR;

namespace DioVehicleApi.Application.Features.Models.Commands.CreateModel;

/// <summary>
/// Handler for creating a model
/// </summary>
public class CreateModelCommandHandler : IRequestHandler<CreateModelCommand, Model>
{
    private readonly IModelRepository _modelRepository;
    private readonly IBrandRepository _brandRepository;

    public CreateModelCommandHandler(IModelRepository modelRepository, IBrandRepository brandRepository)
    {
        _modelRepository = modelRepository;
        _brandRepository = brandRepository;
    }

    public async Task<Model> Handle(CreateModelCommand request, CancellationToken cancellationToken)
    {
        await ValidateIfBrandExistsAndActiveAsync(request.BrandId, cancellationToken);

        var model = new Model
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            BrandId = request.BrandId
        };

        await _modelRepository.CreateAsync(model, cancellationToken);
        await _modelRepository.SaveChangesAsync(cancellationToken);

        return model;
    }

    private async Task ValidateIfBrandExistsAndActiveAsync(Guid brandId, CancellationToken cancellationToken)
    {
        var brand = await _brandRepository.GetByIdAsync(brandId, cancellationToken);

        if (brand == null) throw new NotFoundException(nameof(Brand), brandId);

        if (brand.IsDeleted) throw new DomainException($"Cannot create model for inactive brand '{brand.Name}'");
    }
}
