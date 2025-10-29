using DioVehicleApi.Domain.Entities;
using MediatR;

namespace DioVehicleApi.Application.Features.Brands.Commands.CreateBrand;

public record CreateBrandCommand : IRequest<Brand>
{
    public required string Name { get; init; }
}
