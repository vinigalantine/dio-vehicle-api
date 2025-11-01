using DioVehicleApi.Application.Contracts.Models;
using FluentAssertions;

namespace DioVehicleApi.Application.UnitTests.Contracts.Models;

public class ModelResponseTests
{
    [Fact]
    public void ModelResponse_WithValidProperties_ShouldSetCorrectly()
    {
        var id = Guid.NewGuid();
        var brandId = Guid.NewGuid();
        var response = new ModelResponse
        {
            Id = id,
            Name = "Corolla",
            BrandId = brandId,
        };

        response.Id.Should().Be(id);
        response.Name.Should().Be("Corolla");
        response.BrandId.Should().Be(brandId);
    }
}
