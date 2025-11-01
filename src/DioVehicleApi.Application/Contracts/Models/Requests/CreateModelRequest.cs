namespace DioVehicleApi.Application.Contracts.Models;

/// <summary>
/// Request for creating a model
/// </summary>
public record CreateModelRequest
{
    public required string Name { get; init; }
    public required Guid BrandId { get; init; }
}
