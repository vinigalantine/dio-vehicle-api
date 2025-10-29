namespace DioVehicleApi.Application.Contracts.Brands;

public record BrandResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public DateTime CreatedAt { get; init; }
    public string CreatedBy { get; init; } = string.Empty;
    public DateTime? UpdatedAt { get; init; }
    public string? UpdatedBy { get; init; }
    public DateTime? DeletedAt { get; init; }
    public string? DeletedBy { get; init; }
}
