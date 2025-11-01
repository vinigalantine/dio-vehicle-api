using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Exceptions;
using DioVehicleApi.Domain.Interfaces.Repositories;
using MediatR;

namespace DioVehicleApi.Application.Features.Brands.Commands.UpdateBrand;

/// <summary>
/// Handler for updating a brand
/// </summary>
public class UpdateBrandCommandHandler : IRequestHandler<UpdateBrandCommand, Brand>
{
    private readonly IBrandRepository _brandRepository;

    public UpdateBrandCommandHandler(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public async Task<Brand> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
    {
        var brand = await _brandRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (brand == null) throw new NotFoundException(nameof(Brand), request.Id);

        brand.Name = request.Name;

        var updated = _brandRepository.Update(brand);
        
        await _brandRepository.SaveChangesAsync(cancellationToken);
        
        return updated;
    }
}
