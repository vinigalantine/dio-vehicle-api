namespace DioVehicleApi.Application.Contracts.Base;

/// <summary>
/// Result from a paginated query including items and total count
/// </summary>
public record PaginatedResult<T>
{
    public required IEnumerable<T> Items { get; init; }
    public int TotalCount { get; init; }
}
