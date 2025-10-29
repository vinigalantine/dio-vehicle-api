namespace DioVehicleApi.Application.Contracts.Vehicle;

/// <summary>
/// Request for updating a vehicle
/// </summary>
public record UpdateVehicleRequest
{
    public required int Year { get; init; }
    public required string Color { get; init; }
    public required string LicensePlate { get; init; }
    public required Guid ModelId { get; init; }
}
