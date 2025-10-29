using DioVehicleApi.Domain.Entities;
using MediatR;

namespace DioVehicleApi.Application.Features.Models.Queries.GetModel;

/// <summary>
/// Query to retrieve a model by ID
/// </summary>
public record GetModelQuery : IRequest<Model?>
{
    public required Guid Id { get; init; }
}
