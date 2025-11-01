using DioVehicleApi.Application.Contracts.Base;
using DioVehicleApi.Application.Contracts.Brands;
using DioVehicleApi.Application.Contracts.Models;
using DioVehicleApi.Application.Contracts.Vehicles;
using DioVehicleApi.Application.Features.Auth.Commands.Login;
using DioVehicleApi.Api.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace DioVehicleApi.Api.IntegrationTests.Controllers;

public class VehicleControllerTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private string? _authToken;

    public VehicleControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    public async Task InitializeAsync()
    {
        _authToken = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", _authToken);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetAllVehicles_WithoutAuthentication_ShouldReturn401()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.GetAsync("/api/vehicles");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateVehicle_WithValidData_ShouldReturn201()
    {
        var model = await CreateModelAsync("TestBrand", "TestModel");
        var request = new CreateVehicleRequest
        {
            ModelId = model.Id,
            Year = 2023,
            Color = "Blue",
            LicensePlate = "ABC-1234"
        };

        var response = await _client.PostAsJsonAsync("/api/vehicles", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var vehicle = await response.Content.ReadFromJsonAsync<VehicleResponse>();
        vehicle.Should().NotBeNull();
        vehicle!.Color.Should().Be("Blue");
    }

    [Fact]
    public async Task GetAllVehicles_WithValidRequest_ShouldReturnPagedResult()
    {
        var response = await _client.GetAsync("/api/vehicles?pageNumber=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PaginatedResult<VehicleResponse>>();
        result.Should().NotBeNull();
        result!.Items.Should().NotBeNull();
    }

    [Fact]
    public async Task GetVehicleById_WithExistingId_ShouldReturnVehicle()
    {
        var model = await CreateModelAsync("TestBrand2", "TestModel2");
        var createRequest = new CreateVehicleRequest
        {
            ModelId = model.Id,
            Year = 2023,
            Color = "Red",
            LicensePlate = "XYZ-5678"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/vehicles", createRequest);
        var createdVehicle = await createResponse.Content.ReadFromJsonAsync<VehicleResponse>();

        var response = await _client.GetAsync($"/api/vehicles/{createdVehicle!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var vehicle = await response.Content.ReadFromJsonAsync<VehicleResponse>();
        vehicle.Should().NotBeNull();
        vehicle!.Id.Should().Be(createdVehicle.Id);
    }

    [Fact]
    public async Task UpdateVehicle_WithValidData_ShouldReturn200()
    {
        var model = await CreateModelAsync("TestBrand3", "TestModel3");
        var createRequest = new CreateVehicleRequest
        {
            ModelId = model.Id,
            Year = 2023,
            Color = "Green",
            LicensePlate = "DEF-9012"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/vehicles", createRequest);
        var createdVehicle = await createResponse.Content.ReadFromJsonAsync<VehicleResponse>();

        var updateRequest = new UpdateVehicleRequest
        {
            ModelId = model.Id,
            Year = 2024,
            Color = "Yellow",
            LicensePlate = "DEF-9012"
        };

        var response = await _client.PutAsJsonAsync($"/api/vehicles/{createdVehicle!.Id}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteVehicle_WithExistingId_ShouldReturn204()
    {
        var model = await CreateModelAsync("TestBrand4", "TestModel4");
        var createRequest = new CreateVehicleRequest
        {
            ModelId = model.Id,
            Year = 2023,
            Color = "White",
            LicensePlate = "GHI-3456"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/vehicles", createRequest);
        var createdVehicle = await createResponse.Content.ReadFromJsonAsync<VehicleResponse>();

        var response = await _client.DeleteAsync($"/api/vehicles/{createdVehicle!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private async Task<string> GetAuthTokenAsync()
    {
        var loginRequest = new { Username = "admin", Password = "admin123" };
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        return result!.Token;
    }

    private async Task<ModelResponse> CreateModelAsync(string brandName, string modelName)
    {
        var brandRequest = new { Name = brandName };
        var brandResponse = await _client.PostAsJsonAsync("/api/brands", brandRequest);
        var brand = await brandResponse.Content.ReadFromJsonAsync<BrandResponse>();

        var modelRequest = new { Name = modelName, BrandId = brand!.Id };
        var modelResponse = await _client.PostAsJsonAsync("/api/models", modelRequest);
        return (await modelResponse.Content.ReadFromJsonAsync<ModelResponse>())!;
    }
}
