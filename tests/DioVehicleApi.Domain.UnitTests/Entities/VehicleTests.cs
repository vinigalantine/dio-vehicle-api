using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Interfaces;
using FluentAssertions;

namespace DioVehicleApi.Domain.UnitTests.Entities;

public class VehicleTests
{
    [Fact]
    public void CreateVehicle_WithValidProperties_ShouldSetCorrectly()
    {
        var id = Guid.NewGuid();
        var modelId = Guid.NewGuid();
        var year = 2023;
        var color = "Red";
        var licensePlate = "ABC-1234";

        var vehicle = new Vehicle
        {
            Id = id,
            ModelId = modelId,
            Year = year,
            Color = color,
            LicensePlate = licensePlate
        };

        vehicle.Should().NotBeNull();
        vehicle.Id.Should().Be(id);
        vehicle.ModelId.Should().Be(modelId);
        vehicle.Year.Should().Be(year);
        vehicle.Color.Should().Be(color);
        vehicle.LicensePlate.Should().Be(licensePlate);
        vehicle.IsDeleted.Should().BeFalse();
        vehicle.DeletedAt.Should().BeNull();
        vehicle.DeletedBy.Should().BeNull();
    }

    [Fact]
    public void Vehicle_ShouldHaveModelRelationship()
    {
        var modelId = Guid.NewGuid();
        var model = new Model 
        { 
            Id = modelId, 
            Name = "Civic",
            BrandId = Guid.NewGuid()
        };
        
        var vehicle = new Vehicle
        {
            Id = Guid.NewGuid(),
            ModelId = modelId,
            Model = model,
            Year = 2023,
            Color = "Blue",
            LicensePlate = "XYZ-5678"
        };

        vehicle.Model.Should().NotBeNull();
        vehicle.Model.Should().Be(model);
        vehicle.ModelId.Should().Be(modelId);
    }

    [Fact]
    public void Vehicle_WithDifferentYears_ShouldStoreCorrectly()
    {
        var vehicle = new Vehicle 
        { 
            Id = Guid.NewGuid(),
            ModelId = Guid.NewGuid(),
            Year = 2025,
            Color = "White",
            LicensePlate = "TEST-123"
        };

        vehicle.Year.Should().Be(2025);
    }

    [Fact]
    public void Vehicle_WhenSoftDeleted_ShouldSetAllDeleteProperties()
    {
        var vehicle = new Vehicle 
        { 
            Id = Guid.NewGuid(),
            ModelId = Guid.NewGuid(),
            Year = 2023,
            Color = "Green",
            LicensePlate = "DEL-5555"
        };
        var deletedAt = DateTime.UtcNow;
        var deletedBy = "TestUser";

        vehicle.IsDeleted = true;
        vehicle.DeletedAt = deletedAt;
        vehicle.DeletedBy = deletedBy;

        vehicle.IsDeleted.Should().BeTrue();
        vehicle.DeletedAt.Should().Be(deletedAt);
        vehicle.DeletedBy.Should().Be(deletedBy);
    }

    [Fact]
    public void Vehicle_ShouldImplementISoftDeletable()
    {
        var vehicle = new Vehicle 
        { 
            Id = Guid.NewGuid(),
            ModelId = Guid.NewGuid(),
            Year = 2023,
            Color = "Yellow",
            LicensePlate = "YLW-1111"
        };

        vehicle.Should().BeAssignableTo<ISoftDeletable>();
    }

    [Fact]
    public void Vehicle_ShouldInheritFromBaseEntity()
    {
        var vehicle = new Vehicle 
        { 
            Id = Guid.NewGuid(),
            ModelId = Guid.NewGuid(),
            Year = 2023,
            Color = "Purple",
            LicensePlate = "PRP-2222"
        };

        vehicle.Should().BeAssignableTo<BaseEntity<Guid>>();
    }

    [Fact]
    public void Vehicle_DefaultValues_ShouldBeCorrect()
    {
        var vehicle = new Vehicle { Id = Guid.NewGuid() };

        vehicle.ModelId.Should().Be(Guid.Empty);
        vehicle.Year.Should().Be(0);
        vehicle.Color.Should().BeEmpty();
        vehicle.LicensePlate.Should().BeEmpty();
        vehicle.IsDeleted.Should().BeFalse();
        vehicle.Model.Should().BeNull();
    }
}
