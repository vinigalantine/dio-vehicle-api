namespace DioVehicleApi.Application.Features.BaseFeature.Queries;

/// <summary>
/// Base record for paginated queries
/// </summary>
public abstract record Pagination
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    public int Skip => (PageNumber - 1) * PageSize;
}
