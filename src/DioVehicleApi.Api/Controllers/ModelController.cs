using DioVehicleApi.Api.Constants;
using DioVehicleApi.Application.Contracts.Base;
using DioVehicleApi.Application.Contracts.Model;
using DioVehicleApi.Application.Features.Models.Commands.CreateModel;
using DioVehicleApi.Application.Features.Models.Commands.DeleteModel;
using DioVehicleApi.Application.Features.Models.Commands.UpdateModel;
using DioVehicleApi.Application.Features.Models.Queries.GetAllModels;
using DioVehicleApi.Application.Features.Models.Queries.GetModel;
using DioVehicleApi.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DioVehicleApi.Api.Controllers;

[Route("api/models")]
[ApiController]
public class ModelController : ControllerBase
{
    private readonly ILogger<ModelController> _logger;
    private readonly IMediator _mediator;

    public ModelController(
        ILogger<ModelController> logger,
        IMediator mediator
        )
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Gets all models
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<ModelResponse>>> GetAllModels([FromQuery] string? name, [FromQuery] Guid? brandId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var query = new GetAllModelsQuery()
            {
                Name = name,
                BrandId = brandId,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            var isAdmin = User.IsInRole(ApiConstants.Roles.Admin);

            var items = result.Items.Select(model => new ModelResponse
            {
                Id = model.Id,
                Name = model.Name,
                BrandId = model.BrandId,
                CreatedAt = model.CreatedAt,
                CreatedBy = isAdmin ? model.CreatedBy : ApiConstants.Memes.WeatherBoi,
                UpdatedAt = model.UpdatedAt,
                UpdatedBy = isAdmin ? model.UpdatedBy : ApiConstants.Memes.WeatherBoi,
            });

            var response = new PaginatedResponse<ModelResponse>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = result.TotalCount
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all models.");
            return StatusCode(500, "An error occurred while getting all models.");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ModelResponse>> GetModel(Guid id)
    {
        try
        {
            var query = new GetModelQuery { Id = id };
            var model = await _mediator.Send(query);

            if (model == null) return NotFound($"Model with {id} was not found.");

            var isAdmin = User.IsInRole(ApiConstants.Roles.Admin);

            ModelResponse response = new()
            {
                Id = model.Id,
                Name = model.Name,
                BrandId = model.BrandId,
                CreatedAt = model.CreatedAt,
                CreatedBy = isAdmin ? model.CreatedBy : ApiConstants.Memes.WeatherBoi,
                UpdatedAt = model.UpdatedAt,
                UpdatedBy = isAdmin ? model.UpdatedBy : ApiConstants.Memes.WeatherBoi,
                DeletedAt = model.DeletedAt,
                DeletedBy = isAdmin ? model.DeletedBy : ApiConstants.Memes.WeatherBoi
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting model {ModelId}.", id);
            return StatusCode(500, "An error occurred while getting model.");
        }
    }

    /// <summary>
    /// Creates a new model
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ModelResponse>> CreateModel([FromBody] CreateModelRequest request)
    {
        try
        {
            var command = new CreateModelCommand
            {
                Name = request.Name,
                BrandId = request.BrandId
            };

            var model = await _mediator.Send(command);

            var isAdmin = User.IsInRole(ApiConstants.Roles.Admin);

            var response = new ModelResponse
            {
                Id = model.Id,
                Name = model.Name,
                BrandId = request.BrandId,
                CreatedAt = model.CreatedAt,
            };

            _logger.LogInformation("Model created successfully: {ModelId} - {ModelName}", model.Id, model.Name);
            return CreatedAtAction(nameof(GetModel), new { id = model.Id }, response);
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate key") == true 
                                          || ex.InnerException?.Message.Contains("IX_Models") == true)
        {
            _logger.LogWarning(ex, "Attempt to create model with duplicate name for brand: {ModelName}", request.Name);
            return Conflict(new { message = $"A model with name '{request.Name}' already exists for this brand." });
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Brand not found when creating model: {BrandId}", request.BrandId);
            return NotFound(new { message = ex.Message });
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain validation failed when creating model");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating model with name: {ModelName}", request.Name);
            return StatusCode(500, "An error occurred while creating the model.");
        }
    }

    /// <summary>
    /// Updates a model
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ModelResponse>> UpdateModel(Guid id, [FromBody] UpdateModelRequest request)
    {
        try
        {
            var command = new UpdateModelCommand
            {
                Id = id,
                BrandId = request.BrandId,
                Name = request.Name ?? string.Empty
            };

            var model = await _mediator.Send(command);

            var isAdmin = User.IsInRole(ApiConstants.Roles.Admin);

            var response = new ModelResponse
            {
                Id = model.Id,
                Name = model.Name,
                BrandId = model.BrandId,
                CreatedAt = model.CreatedAt,
                CreatedBy = isAdmin ? model.CreatedBy : ApiConstants.Memes.WeatherBoi,
                UpdatedAt = model.UpdatedAt,
                UpdatedBy = model.UpdatedBy,
            };

            _logger.LogInformation("Model updated successfully: {ModelId}", id);
            return Ok(response);
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate key") == true 
                                          || ex.InnerException?.Message.Contains("IX_Models") == true)
        {
            _logger.LogWarning(ex, "Attempt to update model with duplicate name for brand: {ModelName}", request.Name);
            return Conflict(new { message = $"A model with name '{request.Name}' already exists for this brand." });
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning("Model not found for update: {ModelId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain validation failed when updating model");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating model {ModelId}", id);
            return StatusCode(500, "An error occurred while updating the model.");
        }
    }

    /// <summary>
    /// Deletes a model
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteModel(Guid id)
    {
        try
        {
            var command = new DeleteModelCommand
            {
                Id = id
            };

            var deleted = await _mediator.Send(command);

            if (!deleted)
            {
                _logger.LogWarning("Model not found for deletion: {ModelId}", id);
                return NotFound($"Model with ID {id} not found.");
            }

            _logger.LogInformation("Model deleted successfully: {ModelId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting model {ModelId}", id);
            return StatusCode(500, "An error occurred while deleting the model.");
        }
    }
}

