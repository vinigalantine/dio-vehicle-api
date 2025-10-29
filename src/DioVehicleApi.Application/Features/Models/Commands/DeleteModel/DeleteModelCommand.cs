using MediatR;

namespace DioVehicleApi.Application.Features.Models.Commands.DeleteModel;

/// <summary>
/// Command to delete a model
/// </summary>
public record DeleteModelCommand : IRequest<bool>
{
    public required Guid Id { get; init; }
}
