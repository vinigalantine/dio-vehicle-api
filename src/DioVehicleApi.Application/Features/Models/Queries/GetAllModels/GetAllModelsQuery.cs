using DioVehicleApi.Application.Contracts.Base;
using DioVehicleApi.Application.Features.BaseFeature.Queries;
using DioVehicleApi.Domain.Entities;
using MediatR;

namespace DioVehicleApi.Application.Features.Models.Queries.GetAllModels;

/// <summary>
/// Query to retrieve all models
/// </summary>
public record GetAllModelsQuery : Pagination, IRequest<PaginatedResult<Model>>
{
    public string? Name { get; set; }
    public Guid? BrandId { get; set; }
}
