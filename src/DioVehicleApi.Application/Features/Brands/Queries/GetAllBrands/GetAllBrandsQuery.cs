using DioVehicleApi.Application.Contracts.Base;
using DioVehicleApi.Application.Features.BaseFeature.Queries;
using DioVehicleApi.Domain.Entities;
using MediatR;

namespace DioVehicleApi.Application.Features.Brands.Queries.GetAllBrands;

/// <summary>
/// Query to retrieve all brands
/// </summary>
public record GetAllBrandsQuery : Pagination, IRequest<PaginatedResult<Brand>>
{
    public string? Name { get; set; }
}
