using DioVehicleApi.Domain.Entities;
using MediatR;

namespace DioVehicleApi.Application.Features.Brands.Commands.UpdateBrand;

/// <summary>
/// Command to update a brand
/// </summary>
public record UpdateBrandCommand : IRequest<Brand>
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}
