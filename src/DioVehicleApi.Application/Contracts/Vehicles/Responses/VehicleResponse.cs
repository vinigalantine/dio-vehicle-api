namespace DioVehicleApi.Application.Contracts.Vehicles;

public record VehicleResponse
{
    public required Guid Id { get; init; }
    public int Year { get; set; }
    public required string Color { get; set; }
    public required string LicensePlate { get; set; }
    public required Guid ModelId { get; init; }
    public DateTime CreatedAt { get; init; }
    public string CreatedBy { get; init; } = string.Empty;
    public DateTime? UpdatedAt { get; init; }
    public string? UpdatedBy { get; init; }
    public DateTime? DeletedAt { get; init; }
    public string? DeletedBy { get; init; }
}
