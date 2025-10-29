namespace DioVehicleApi.Application.Contracts.Brands;

/// <summary>
/// Request for updating a brand
/// </summary>
public record UpdateBrandRequest
{
    public string? Name { get; init; }
}
