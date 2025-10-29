using MediatR;

namespace DioVehicleApi.Application.Features.Brands.Commands.DeleteBrand;

/// <summary>
/// Command to delete a brand
/// </summary>
public record DeleteBrandCommand : IRequest<bool>
{
    public required Guid Id { get; init; }
}
