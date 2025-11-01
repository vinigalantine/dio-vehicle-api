using DioVehicleApi.Domain.Entities;
using MediatR;

namespace DioVehicleApi.Application.Features.Brands.Queries.GetBrand;

/// <summary>
/// Query to retrieve a single brand by ID
/// </summary>
public record GetBrandQuery : IRequest<Brand?>
{
    public required Guid Id { get; init; }
}
