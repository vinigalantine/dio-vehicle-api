using DioVehicleApi.Domain.Interfaces.Repositories;
using MediatR;

namespace DioVehicleApi.Application.Features.Brands.Commands.DeleteBrand;

/// <summary>
/// Handler for deleting a brand
/// </summary>
public class DeleteBrandCommandHandler : IRequestHandler<DeleteBrandCommand, bool>
{
    private readonly IBrandRepository _brandRepository;

    public DeleteBrandCommandHandler(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public async Task<bool> Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
    {
        var brand = await _brandRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (brand == null)
        {
            return false;
        }

        _brandRepository.Remove(brand);
        var rowsAffected = await _brandRepository.SaveChangesAsync(cancellationToken);
        
        return rowsAffected > 0;
    }
}
