using System.Linq.Expressions;
using DioVehicleApi.Application.Contracts.Base;
using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Interfaces.Repositories;
using MediatR;

namespace DioVehicleApi.Application.Features.Models.Queries.GetAllModels;

/// <summary>
/// Handler for retrieving all models
/// </summary>
public class GetAllModelsQueryHandler : IRequestHandler<GetAllModelsQuery, PaginatedResult<Model>>
{
    private readonly IModelRepository _modelRepository;

    public GetAllModelsQueryHandler(IModelRepository modelRepository)
    {
        _modelRepository = modelRepository;
    }

    public async Task<PaginatedResult<Model>> Handle(GetAllModelsQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Model, bool>> filter = model => 
            (string.IsNullOrEmpty(request.Name) || model.Name.Contains(request.Name)) &&
            (!request.BrandId.HasValue || model.BrandId == request.BrandId.Value);

        var models = await _modelRepository.GetAllAsync(
            filter: filter,
            skip: (request.PageNumber - 1) * request.PageSize, 
            take: request.PageSize, 
            cancellationToken: cancellationToken
        );

        var totalCount = await _modelRepository.CountAsync(
            filter: filter,
            cancellationToken: cancellationToken
        );

        return new PaginatedResult<Model>
        {
            Items = models,
            TotalCount = totalCount
        };
    }
}

