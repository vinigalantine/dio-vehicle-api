using DioVehicleApi.Api.IntegrationTests.Infrastructure;
using DioVehicleApi.Application.Contracts.Base;
using DioVehicleApi.Application.Contracts.Brands;
using DioVehicleApi.Application.Features.Auth.Commands.Login;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace DioVehicleApi.Api.IntegrationTests.Controllers;

public class BrandControllerTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private string? _authToken;

    public BrandControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    public async Task InitializeAsync()
    {
        // Authenticate before each test
        _authToken = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", _authToken);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetAllBrands_WithoutAuthentication_ShouldReturn401()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.GetAsync("/api/brands");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAllBrands_WithAuthentication_ShouldReturn200()
    {
        var response = await _client.GetAsync("/api/brands?pageNumber=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetAllBrands_ShouldReturnPaginatedResponse()
    {
        var response = await _client.GetAsync("/api/brands?pageNumber=1&pageSize=10");
        var result = await response.Content.ReadFromJsonAsync<PaginatedResult<BrandResponse>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Items.Should().NotBeNull();
        result.TotalCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task CreateBrand_ValidData_ShouldReturn201()
    {
        var request = new CreateBrandRequest { Name = $"TestBrand_{Guid.NewGuid()}" };

        var response = await _client.PostAsJsonAsync("/api/brands", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<BrandResponse>();
        created.Should().NotBeNull();
        created!.Name.Should().Be(request.Name);
        created.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateBrand_DuplicateName_ShouldReturn409()
    {
        var brandName = $"DuplicateTest_{Guid.NewGuid()}";
        var request1 = new CreateBrandRequest { Name = brandName };
        var request2 = new CreateBrandRequest { Name = brandName };

        await _client.PostAsJsonAsync("/api/brands", request1);
        var response = await _client.PostAsJsonAsync("/api/brands", request2);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CreateBrand_EmptyName_ShouldReturn400()
    {
        var request = new CreateBrandRequest { Name = "" };

        var response = await _client.PostAsJsonAsync("/api/brands", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetBrand_ExistingId_ShouldReturn200()
    {
        var created = await CreateBrandAsync($"GetTest_{Guid.NewGuid()}");

        var response = await _client.GetAsync($"/api/brands/{created.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var brand = await response.Content.ReadFromJsonAsync<BrandResponse>();
        brand.Should().NotBeNull();
        brand!.Id.Should().Be(created.Id);
        brand.Name.Should().Be(created.Name);
    }

    [Fact]
    public async Task GetBrand_NonExistingId_ShouldReturn404()
    {
        var nonExistingId = Guid.NewGuid();

        var response = await _client.GetAsync($"/api/brands/{nonExistingId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateBrand_ValidData_ShouldReturn200()
    {
        var created = await CreateBrandAsync($"UpdateTest_{Guid.NewGuid()}");

        var updateRequest = new UpdateBrandRequest { Name = $"Updated_{Guid.NewGuid()}" };

        var response = await _client.PutAsJsonAsync($"/api/brands/{created.Id}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<BrandResponse>();
        updated.Should().NotBeNull();
        updated!.Name.Should().Be(updateRequest.Name);
    }

    [Fact]
    public async Task UpdateBrand_NonExistingId_ShouldReturn404()
    {
        var nonExistingId = Guid.NewGuid();
        var updateRequest = new UpdateBrandRequest { Name = "NewName" };

        var response = await _client.PutAsJsonAsync($"/api/brands/{nonExistingId}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteBrand_ExistingId_ShouldReturn204()
    {
        var created = await CreateBrandAsync($"DeleteTest_{Guid.NewGuid()}");

        var response = await _client.DeleteAsync($"/api/brands/{created.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/brands/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteBrand_NonExistingId_ShouldReturn404()
    {
        var nonExistingId = Guid.NewGuid();

        var response = await _client.DeleteAsync($"/api/brands/{nonExistingId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
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
