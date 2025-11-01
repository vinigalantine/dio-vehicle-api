using DioVehicleApi.Application.Contracts.Vehicles;
using FluentAssertions;

namespace DioVehicleApi.Application.UnitTests.Contracts.Vehicles;

public class VehicleResponseTests
{
    [Fact]
    public void VehicleResponse_WithValidProperties_ShouldSetCorrectly()
    {
        var id = Guid.NewGuid();
        var modelId = Guid.NewGuid();
        var response = new VehicleResponse
        {
            Id = id,
            ModelId = modelId,
            Year = 2023,
            Color = "White",
            LicensePlate = "ABC-1234"
        };

        response.Id.Should().Be(id);
        response.ModelId.Should().Be(modelId);
        response.Year.Should().Be(2023);
        response.Color.Should().Be("White");
        response.LicensePlate.Should().Be("ABC-1234");
    }
}
