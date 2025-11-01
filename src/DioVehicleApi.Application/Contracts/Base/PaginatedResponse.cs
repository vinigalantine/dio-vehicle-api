namespace DioVehicleApi.Application.Contracts.Base;

public record PaginatedResponse<T>
{
    public required IEnumerable<T> Items { get; set; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public int TotalCount { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
