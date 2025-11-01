using System.Linq.Expressions;
using DioVehicleApi.Application.Contracts.Base;
using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Interfaces.Repositories;
using MediatR;

namespace DioVehicleApi.Application.Features.Brands.Queries.GetAllBrands;

/// <summary>
/// Handler for retrieving all brands
/// </summary>
public class GetAllBrandsQueryHandler : IRequestHandler<GetAllBrandsQuery, PaginatedResult<Brand>>
{
    private readonly IBrandRepository _brandRepository;

    public GetAllBrandsQueryHandler(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public async Task<PaginatedResult<Brand>> Handle(GetAllBrandsQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Brand, bool>> filter = brand => string.IsNullOrEmpty(request.Name) || brand.Name.Contains(request.Name);

        var brands = await _brandRepository.GetAllAsync(
            filter: filter,
            skip: (request.PageNumber - 1) * request.PageSize, 
            take: request.PageSize, 
            cancellationToken: cancellationToken
        );

        var totalCount = await _brandRepository.CountAsync(
            filter: filter,
            cancellationToken: cancellationToken
        );

        return new PaginatedResult<Brand>
        {
            Items = brands,
            TotalCount = totalCount
        };
    }
}
