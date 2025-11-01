using DioVehicleApi.Application.Contracts.Base;
using DioVehicleApi.Application.Contracts.Brands;
using DioVehicleApi.Application.Contracts.Models;
using DioVehicleApi.Application.Features.Auth.Commands.Login;
using DioVehicleApi.Api.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace DioVehicleApi.Api.IntegrationTests.Controllers;

public class ModelControllerTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private string? _authToken;

    public ModelControllerTests(CustomWebApplicationFactory factory)
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
    public async Task GetAllModels_WithoutAuthentication_ShouldReturn401()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.GetAsync("/api/models");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateModel_WithValidData_ShouldReturn201()
    {
        var brandResponse = await CreateBrandAsync("TestBrand");
        var request = new CreateModelRequest
        {
            Name = "TestModel",
            BrandId = brandResponse.Id
        };

        var response = await _client.PostAsJsonAsync("/api/models", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var model = await response.Content.ReadFromJsonAsync<ModelResponse>();
        model.Should().NotBeNull();
        model!.Name.Should().Be("TestModel");
    }

    [Fact]
    public async Task GetAllModels_WithValidRequest_ShouldReturnPagedResult()
    {
        var response = await _client.GetAsync("/api/models?pageNumber=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PaginatedResult<ModelResponse>>();
        result.Should().NotBeNull();
        result!.Items.Should().NotBeNull();
    }

    [Fact]
    public async Task GetModelById_WithExistingId_ShouldReturnModel()
    {
        var brandResponse = await CreateBrandAsync("TestBrand2");
        var createRequest = new CreateModelRequest
        {
            Name = "TestModel2",
            BrandId = brandResponse.Id
        };
        var createResponse = await _client.PostAsJsonAsync("/api/models", createRequest);
        var createdModel = await createResponse.Content.ReadFromJsonAsync<ModelResponse>();

        var response = await _client.GetAsync($"/api/models/{createdModel!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var model = await response.Content.ReadFromJsonAsync<ModelResponse>();
        model.Should().NotBeNull();
        model!.Id.Should().Be(createdModel.Id);
    }

    [Fact]
    public async Task UpdateModel_WithValidData_ShouldReturn200()
    {
        var brandResponse = await CreateBrandAsync("TestBrand3");
        var createRequest = new CreateModelRequest
        {
            Name = "TestModel3",
            BrandId = brandResponse.Id
        };
        var createResponse = await _client.PostAsJsonAsync("/api/models", createRequest);
        var createdModel = await createResponse.Content.ReadFromJsonAsync<ModelResponse>();

        var updateRequest = new UpdateModelRequest
        {
            Name = "UpdatedModel",
            BrandId = brandResponse.Id
        };

        var response = await _client.PutAsJsonAsync($"/api/models/{createdModel!.Id}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteModel_WithExistingId_ShouldReturn204()
    {
        var brandResponse = await CreateBrandAsync("TestBrand4");
        var createRequest = new CreateModelRequest
        {
            Name = "TestModel4",
            BrandId = brandResponse.Id
        };
        var createResponse = await _client.PostAsJsonAsync("/api/models", createRequest);
        var createdModel = await createResponse.Content.ReadFromJsonAsync<ModelResponse>();

        var response = await _client.DeleteAsync($"/api/models/{createdModel!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private async Task<string> GetAuthTokenAsync()
    {
        var loginRequest = new { Username = "admin", Password = "admin123" };
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        return result!.Token;
    }

    private async Task<BrandResponse> CreateBrandAsync(string name)
    {
        var request = new CreateBrandRequest { Name = name };
        var response = await _client.PostAsJsonAsync("/api/brands", request);
        return (await response.Content.ReadFromJsonAsync<BrandResponse>())!;
    }
}
