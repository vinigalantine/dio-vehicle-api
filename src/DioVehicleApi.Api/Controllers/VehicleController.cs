using DioVehicleApi.Api.Constants;
using DioVehicleApi.Application.Contracts.Base;
using DioVehicleApi.Application.Contracts.Vehicles;
using DioVehicleApi.Application.Features.Vehicles.Commands.CreateVehicle;
using DioVehicleApi.Application.Features.Vehicles.Commands.DeleteVehicle;
using DioVehicleApi.Application.Features.Vehicles.Commands.UpdateVehicle;
using DioVehicleApi.Application.Features.Vehicles.Queries.GetAllVehicles;
using DioVehicleApi.Application.Features.Vehicles.Queries.GetVehicle;
using DioVehicleApi.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DioVehicleApi.Api.Controllers;

[Route("api/vehicles")]
[ApiController]
[Authorize]
public class VehicleController : ControllerBase
{
    private readonly ILogger<VehicleController> _logger;
    private readonly IMediator _mediator;

    public VehicleController(
        ILogger<VehicleController> logger,
        IMediator mediator
        )
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Gets all vehicles
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VehicleResponse>>> GetAllVehicles([FromQuery] int? year,[FromQuery] string? color,[FromQuery] string? licensePlate, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var query = new GetAllVehiclesQuery()
            {
                Year = year,
                Color = color,
                LicensePlate = licensePlate,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            var isAdmin = User.IsInRole(ApiConstants.Roles.Admin);
            
            var items = result.Items.Select(vehicle => new VehicleResponse
            {
                Id = vehicle.Id,
                Year = vehicle.Year,
                Color = vehicle.Color,
                LicensePlate = vehicle.LicensePlate,
                ModelId = vehicle.ModelId,
                CreatedAt = vehicle.CreatedAt,
                CreatedBy = isAdmin ? vehicle.CreatedBy : ApiConstants.Memes.WeatherBoi,
                UpdatedAt = vehicle.UpdatedAt,
                UpdatedBy = isAdmin ? vehicle.UpdatedBy : ApiConstants.Memes.WeatherBoi,
            });

            var response = new PaginatedResponse<VehicleResponse>
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
            _logger.LogError(ex, "Error getting all vehicles.");
            return StatusCode(500, "An error occurred while getting all vehicles.");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VehicleResponse>> GetVehicle(Guid id)
    {
        try
        {
            var query = new GetVehicleQuery { Id = id };
            var vehicle = await _mediator.Send(query);

            if (vehicle == null) return NotFound($"Vehicle with {id} was not found.");

            var isAdmin = User.IsInRole(ApiConstants.Roles.Admin);

            VehicleResponse response = new()
            {
                Id = vehicle.Id,
                Year = vehicle.Year,
                Color = vehicle.Color,
                LicensePlate = vehicle.LicensePlate,
                ModelId = vehicle.ModelId,
                CreatedAt = vehicle.CreatedAt,
                CreatedBy = isAdmin ? vehicle.CreatedBy : ApiConstants.Memes.WeatherBoi,
                UpdatedAt = vehicle.UpdatedAt,
                UpdatedBy = isAdmin ? vehicle.UpdatedBy : ApiConstants.Memes.WeatherBoi,
                DeletedAt = vehicle.DeletedAt,
                DeletedBy = isAdmin ? vehicle.DeletedBy : ApiConstants.Memes.WeatherBoi
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting vehicle {VehicleId}.", id);
            return StatusCode(500, "An error occurred while getting vehicle.");
        }
    }

    /// <summary>
    /// Creates a new vehicle
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<VehicleResponse>> CreateVehicle([FromBody] CreateVehicleRequest request)
    {
        try
        {
            var command = new CreateVehicleCommand
            {
                Year = request.Year,
                Color = request.Color,
                LicensePlate = request.LicensePlate,
                ModelId = request.ModelId,
            };

            var vehicle = await _mediator.Send(command);

            var response = new VehicleResponse
            {
                Id = vehicle.Id,
                Year = vehicle.Year,
                Color = vehicle.Color,
                LicensePlate = vehicle.LicensePlate,
                ModelId = vehicle.ModelId,
                CreatedAt = vehicle.CreatedAt,
            };

            _logger.LogInformation("Vehicle created successfully: {VehicleId} - {LicensePlate}", vehicle.Id, vehicle.LicensePlate);
            return CreatedAtAction(nameof(GetVehicle), new { id = vehicle.Id }, response);
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate key") == true 
                                          || ex.InnerException?.Message.Contains("IX_Vehicles_LicensePlate") == true)
        {
            _logger.LogWarning(ex, "Attempt to create vehicle with duplicate license plate: {LicensePlate}", request.LicensePlate);
            return Conflict(new { message = $"A vehicle with license plate '{request.LicensePlate}' already exists." });
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Model not found when creating vehicle: {ModelId}", request.ModelId);
            return NotFound(new { message = ex.Message });
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain validation error when creating vehicle: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating vehicle with license plate: {LicensePlate}", request.LicensePlate);
            return StatusCode(500, "An error occurred while creating the vehicle.");
        }
    }

    /// <summary>
    /// Updates a vehicle
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<VehicleResponse>> UpdateVehicle(Guid id, [FromBody] UpdateVehicleRequest request)
    {
        try
        {
            var command = new UpdateVehicleCommand
            {
                Id = id,
                Year = request.Year,
                Color = request.Color,
                LicensePlate = request.LicensePlate,
                ModelId = request.ModelId,
            };

            var vehicle = await _mediator.Send(command);
            
            var isAdmin = User.IsInRole(ApiConstants.Roles.Admin);

            var response = new VehicleResponse
            {
                Id = vehicle.Id,
                Year = vehicle.Year,
                Color = vehicle.Color,
                LicensePlate = vehicle.LicensePlate,
                ModelId = vehicle.ModelId,
                CreatedAt = vehicle.CreatedAt,
                CreatedBy = isAdmin ? vehicle.CreatedBy : ApiConstants.Memes.WeatherBoi,
                UpdatedAt = vehicle.UpdatedAt,
                UpdatedBy = vehicle.UpdatedBy,
            };

            _logger.LogInformation("Vehicle updated successfully: {VehicleId}", id);
            return Ok(response);
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate key") == true 
                                          || ex.InnerException?.Message.Contains("IX_Vehicles_LicensePlate") == true)
        {
            _logger.LogWarning(ex, "Attempt to update vehicle with duplicate license plate: {LicensePlate}", request.LicensePlate);
            return Conflict(new { message = $"A vehicle with license plate '{request.LicensePlate}' already exists." });
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning("Vehicle not found for update: {VehicleId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain validation error when updating vehicle: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating vehicle {VehicleId}", id);
            return StatusCode(500, "An error occurred while updating the vehicle.");
        }
    }

    /// <summary>
    /// Deletes a vehicle
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteVehicle(Guid id)
    {
        try
        {
            var command = new DeleteVehicleCommand
            {
                Id = id
            };

            var deleted = await _mediator.Send(command);

            if (!deleted)
            {
                _logger.LogWarning("Vehicle not found for deletion: {VehicleId}", id);
                return NotFound($"Vehicle with ID {id} not found.");
            }

            _logger.LogInformation("Vehicle deleted successfully: {VehicleId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting vehicle {VehicleId}", id);
            return StatusCode(500, "An error occurred while deleting the vehicle.");
        }
    }
}

