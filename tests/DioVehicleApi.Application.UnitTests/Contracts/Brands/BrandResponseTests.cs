using DioVehicleApi.Application.Contracts.Brands;
using FluentAssertions;

namespace DioVehicleApi.Application.UnitTests.Contracts.Brands;

public class BrandResponseTests
{
    [Fact]
    public void BrandResponse_ShouldSetCorrectly()
    {
        var id = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var response = new BrandResponse
        {
            Id = id,
            Name = "Toyota",
            CreatedAt = now,
            CreatedBy = "TestUser",
            UpdatedAt = now.AddMinutes(10),
            UpdatedBy = "TestUser",
            DeletedAt = now.AddMinutes(20),
            DeletedBy = "Admin"
        };

        response.Id.Should().Be(id);
        response.Name.Should().Be("Toyota");
        response.CreatedAt.Should().Be(now);
        response.CreatedBy.Should().Be("TestUser");
        response.UpdatedAt.Should().Be(now.AddMinutes(10));
        response.UpdatedBy.Should().Be("TestUser");
        response.DeletedAt.Should().Be(now.AddMinutes(20));
        response.DeletedBy.Should().Be("Admin");
    }
}
