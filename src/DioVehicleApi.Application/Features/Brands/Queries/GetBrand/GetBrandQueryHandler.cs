using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Interfaces.Repositories;
using MediatR;

namespace DioVehicleApi.Application.Features.Brands.Queries.GetBrand;

public class GetBrandQueryHandler : IRequestHandler<GetBrandQuery, Brand?>
{
    private readonly IBrandRepository _brandRepository;

    public GetBrandQueryHandler(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public async Task<Brand?> Handle(GetBrandQuery request, CancellationToken cancellationToken)
    {
        return await _brandRepository.GetByIdAsync(request.Id, cancellationToken);
    }
}
