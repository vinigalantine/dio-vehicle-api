using DioVehicleApi.Api.Constants;
using DioVehicleApi.Application.Contracts.Base;
using DioVehicleApi.Application.Contracts.Brands;
using DioVehicleApi.Application.Features.Brands.Commands.CreateBrand;
using DioVehicleApi.Application.Features.Brands.Commands.DeleteBrand;
using DioVehicleApi.Application.Features.Brands.Commands.UpdateBrand;
using DioVehicleApi.Application.Features.Brands.Queries.GetAllBrands;
using DioVehicleApi.Application.Features.Brands.Queries.GetBrand;
using DioVehicleApi.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DioVehicleApi.Api.Controllers;

[Route("api/brands")]
[ApiController]
public class BrandController : ControllerBase
{
    private readonly ILogger<BrandController> _logger;
    private readonly IMediator _mediator;

    public BrandController(
        ILogger<BrandController> logger,
        IMediator mediator
        )
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Gets all brands
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<BrandResponse>>> GetAllBrands([FromQuery] string? name, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var query = new GetAllBrandsQuery()
            {
                Name = name,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var isAdmin = User.IsInRole(ApiConstants.Roles.Admin);

            var result = await _mediator.Send(query);

            var items = result.Items.Select(brand => new BrandResponse
            {
                Id = brand.Id,
                Name = brand.Name,
                CreatedAt = brand.CreatedAt,
                CreatedBy = isAdmin ? brand.CreatedBy : ApiConstants.Memes.WeatherBoi,
                UpdatedAt = brand.UpdatedAt,
                UpdatedBy = isAdmin ? brand.UpdatedBy : ApiConstants.Memes.WeatherBoi
            });

            var response = new PaginatedResponse<BrandResponse>
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
            _logger.LogError(ex, "Error getting all brands.");
            return StatusCode(500, "An error occurred while getting all brands.");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BrandResponse>> GetBrand(Guid id)
    {
        try
        {
            var query = new GetBrandQuery { Id = id };
            var brand = await _mediator.Send(query);

            if (brand == null) return NotFound($"Brand with {id} was not found.");

            var isAdmin = User.IsInRole(ApiConstants.Roles.Admin);

            BrandResponse response = new()
            {
                Id = brand.Id,
                Name = brand.Name,
                CreatedAt = brand.CreatedAt,
                CreatedBy = isAdmin ? brand.CreatedBy : ApiConstants.Memes.WeatherBoi,
                UpdatedAt = brand.UpdatedAt,
                UpdatedBy = isAdmin ? brand.UpdatedBy : ApiConstants.Memes.WeatherBoi,
                DeletedAt = brand.DeletedAt,
                DeletedBy = isAdmin ? brand.DeletedBy : ApiConstants.Memes.WeatherBoi
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting brand {BrandId}.", id);
            return StatusCode(500, "An error occurred while getting brand.");
        }
    }

    /// <summary>
    /// Creates a new brand
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<BrandResponse>> CreateBrand([FromBody] CreateBrandRequest request)
    {
        try
        {
            var command = new CreateBrandCommand
            {
                Name = request.Name,
            };

            var brand = await _mediator.Send(command);

            var isAdmin = User.IsInRole(ApiConstants.Roles.Admin);

            var response = new BrandResponse
            {
                Id = brand.Id,
                Name = brand.Name,
                CreatedAt = brand.CreatedAt,
            };

            _logger.LogInformation("Brand created successfully: {BrandId} - {BrandName}", brand.Id, brand.Name);
            return CreatedAtAction(nameof(GetBrand), new { id = brand.Id }, response);
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate key") == true 
                                          || ex.InnerException?.Message.Contains("IX_Brands_Name") == true)
        {
            _logger.LogWarning(ex, "Attempt to create brand with duplicate name: {BrandName}", request.Name);
            return Conflict(new { message = $"A brand with name '{request.Name}' already exists." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating brand with name: {BrandName}", request.Name);
            return StatusCode(500, "An error occurred while creating the brand.");
        }
    }

    /// <summary>
    /// Updates a brand
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<BrandResponse>> UpdateBrand(Guid id, [FromBody] UpdateBrandRequest request)
    {
        try
        {
            var command = new UpdateBrandCommand
            {
                Id = id,
                Name = request.Name ?? string.Empty
            };

            var brand = await _mediator.Send(command);

            var isAdmin = User.IsInRole(ApiConstants.Roles.Admin);

            var response = new BrandResponse
            {
                Id = brand.Id,
                Name = brand.Name,
                CreatedAt = brand.CreatedAt,
                CreatedBy = isAdmin ? brand.CreatedBy : ApiConstants.Memes.WeatherBoi,
                UpdatedAt = brand.UpdatedAt,
                UpdatedBy = brand.UpdatedBy
            };

            _logger.LogInformation("Brand updated successfully: {BrandId}", id);
            return Ok(response);
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate key") == true 
                                          || ex.InnerException?.Message.Contains("IX_Brands_Name") == true)
        {
            _logger.LogWarning(ex, "Attempt to update brand with duplicate name: {BrandName}", request.Name);
            return Conflict(new { message = $"A brand with name '{request.Name}' already exists." });
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning("Brand not found for update: {BrandId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating brand {BrandId}", id);
            return StatusCode(500, "An error occurred while updating the brand.");
        }
    }

    /// <summary>
    /// Deletes a brand
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteBrand(Guid id)
    {
        try
        {
            var command = new DeleteBrandCommand
            {
                Id = id
            };

            var deleted = await _mediator.Send(command);

            if (!deleted)
            {
                _logger.LogWarning("Brand not found for deletion: {BrandId}", id);
                return NotFound($"Brand with ID {id} not found.");
            }

            _logger.LogInformation("Brand deleted successfully: {BrandId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting brand {BrandId}", id);
            return StatusCode(500, "An error occurred while deleting the brand.");
        }
    }
};
