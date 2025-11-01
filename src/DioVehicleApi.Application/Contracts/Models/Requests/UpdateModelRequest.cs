namespace DioVehicleApi.Application.Contracts.Models;

/// <summary>
/// Request for updating a model
/// </summary>
public record UpdateModelRequest
{
    public required string Name { get; init; }
    public required Guid BrandId { get; init; }
}
