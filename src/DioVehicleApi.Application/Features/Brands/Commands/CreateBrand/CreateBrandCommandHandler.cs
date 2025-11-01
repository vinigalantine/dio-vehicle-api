using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Exceptions;
using DioVehicleApi.Domain.Interfaces.Repositories;
using MediatR;

namespace DioVehicleApi.Application.Features.Brands.Commands.CreateBrand;

/// <summary>
/// Handler for creating a brand
/// </summary>
public class CreateBrandCommandHandler : IRequestHandler<CreateBrandCommand, Brand>
{
    private readonly IBrandRepository _brandRepository;

    public CreateBrandCommandHandler(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public async Task<Brand> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
    {
        if (await _brandRepository.ExistsByNameAsync(request.Name, cancellationToken))
        {
            throw new ConflictException($"A brand with name '{request.Name}' already exists.");
        }

        var brand = new Brand
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
        };

        await _brandRepository.CreateAsync(brand, cancellationToken);
        await _brandRepository.SaveChangesAsync(cancellationToken);

        return brand;
    }
}
