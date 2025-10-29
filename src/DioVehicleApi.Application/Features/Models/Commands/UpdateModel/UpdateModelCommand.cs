using DioVehicleApi.Domain.Entities;
using MediatR;

namespace DioVehicleApi.Application.Features.Models.Commands.UpdateModel;

/// <summary>
/// Command to update a model
/// </summary>
public record UpdateModelCommand : IRequest<Model>
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required Guid BrandId { get; init; }
}
