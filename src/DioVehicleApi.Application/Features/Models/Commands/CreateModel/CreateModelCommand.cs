using DioVehicleApi.Domain.Entities;
using MediatR;

namespace DioVehicleApi.Application.Features.Models.Commands.CreateModel;

public record CreateModelCommand : IRequest<Model>
{
    public required string Name { get; init; }
    public required Guid BrandId { get; init; }
}
