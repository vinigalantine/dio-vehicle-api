using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Interfaces.Repositories;
using MediatR;

namespace DioVehicleApi.Application.Features.Models.Queries.GetModel;

public class GetModelQueryHandler : IRequestHandler<GetModelQuery, Model?>
{
    private readonly IModelRepository _modelRepository;

    public GetModelQueryHandler(IModelRepository modelRepository)
    {
        _modelRepository = modelRepository;
    }

    public async Task<Model?> Handle(GetModelQuery request, CancellationToken cancellationToken)
    {

        return await _modelRepository.GetByIdAsync(request.Id, cancellationToken);
    }
}
