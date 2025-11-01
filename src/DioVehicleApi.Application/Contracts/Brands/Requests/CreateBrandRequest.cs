namespace DioVehicleApi.Application.Contracts.Brands;

/// <summary>
/// Request for creating a brand
/// </summary>
public record CreateBrandRequest
{
    public required string Name { get; init; }
}
