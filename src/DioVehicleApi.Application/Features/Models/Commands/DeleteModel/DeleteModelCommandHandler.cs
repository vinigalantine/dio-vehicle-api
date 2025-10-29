using DioVehicleApi.Domain.Interfaces.Repositories;
using MediatR;

namespace DioVehicleApi.Application.Features.Models.Commands.DeleteModel;

/// <summary>
/// Handler for deleting a model
/// </summary>
public class DeleteModelCommandHandler : IRequestHandler<DeleteModelCommand, bool>
{
    private readonly IModelRepository _modelRepository;

    public DeleteModelCommandHandler(IModelRepository modelRepository)
    {
        _modelRepository = modelRepository;
    }

    public async Task<bool> Handle(DeleteModelCommand request, CancellationToken cancellationToken)
    {
        var model = await _modelRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (model == null)
        {
            return false;
        }

        _modelRepository.Remove(model);
        var rowsAffected = await _modelRepository.SaveChangesAsync(cancellationToken);
        
        return rowsAffected > 0;
    }
}
