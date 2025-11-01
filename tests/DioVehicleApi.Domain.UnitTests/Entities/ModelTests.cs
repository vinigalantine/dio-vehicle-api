using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Interfaces;
using FluentAssertions;

namespace DioVehicleApi.Domain.UnitTests.Entities;

public class ModelTests
{
    [Fact]
    public void CreateModel_WithValidProperties_ShouldSetCorrectly()
    {
        var id = Guid.NewGuid();
        var brandId = Guid.NewGuid();
        var name = "Civic";

        var model = new Model
        {
            Id = id,
            Name = name,
            BrandId = brandId
        };

        model.Should().NotBeNull();
        model.Id.Should().Be(id);
        model.Name.Should().Be(name);
        model.BrandId.Should().Be(brandId);
        model.Vehicles.Should().NotBeNull().And.BeEmpty();
        model.IsDeleted.Should().BeFalse();
        model.DeletedAt.Should().BeNull();
        model.DeletedBy.Should().BeNull();
    }

    [Fact]
    public void Model_ShouldAllowAddingVehicles()
    {
        var model = new Model 
        { 
            Id = Guid.NewGuid(), 
            Name = "Corolla",
            BrandId = Guid.NewGuid()
        };
        var vehicle = new Vehicle 
        { 
            Id = Guid.NewGuid(),
            ModelId = model.Id,
            Year = 2023,
            Color = "White",
            LicensePlate = "ABC-1234"
        };

        model.Vehicles.Add(vehicle);

        model.Vehicles.Should().HaveCount(1);
        model.Vehicles.Should().Contain(vehicle);
        model.Vehicles.First().ModelId.Should().Be(model.Id);
    }

    [Fact]
    public void Model_ShouldHaveBrandRelationship()
    {
        var brandId = Guid.NewGuid();
        var brand = new Brand { Id = brandId, Name = "Honda" };
        
        var model = new Model
        {
            Id = Guid.NewGuid(),
            Name = "Accord",
            BrandId = brandId,
            Brand = brand
        };

        model.Brand.Should().NotBeNull();
        model.Brand.Should().Be(brand);
        model.BrandId.Should().Be(brandId);
    }

    [Theory]
    [InlineData("Civic")]
    [InlineData("Corolla")]
    [InlineData("Mustang")]
    [InlineData("Model S")]
    public void Model_WithDifferentNames_ShouldStoreCorrectly(string name)
    {
        var model = new Model 
        { 
            Id = Guid.NewGuid(), 
            Name = name,
            BrandId = Guid.NewGuid()
        };

        model.Name.Should().Be(name);
        model.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Model_WhenSoftDeleted_ShouldSetAllDeleteProperties()
    {
        var model = new Model 
        { 
            Id = Guid.NewGuid(), 
            Name = "Camry",
            BrandId = Guid.NewGuid()
        };
        var deletedAt = DateTime.UtcNow;
        var deletedBy = "TestUser";

        model.IsDeleted = true;
        model.DeletedAt = deletedAt;
        model.DeletedBy = deletedBy;

        model.IsDeleted.Should().BeTrue();
        model.DeletedAt.Should().Be(deletedAt);
        model.DeletedBy.Should().Be(deletedBy);
    }

    [Fact]
    public void Model_ShouldImplementISoftDeletable()
    {
        var model = new Model 
        { 
            Id = Guid.NewGuid(), 
            Name = "A4",
            BrandId = Guid.NewGuid()
        };

        model.Should().BeAssignableTo<ISoftDeletable>();
    }

    [Fact]
    public void Model_ShouldInheritFromBaseEntity()
    {
        var model = new Model 
        { 
            Id = Guid.NewGuid(), 
            Name = "X5",
            BrandId = Guid.NewGuid()
        };

        model.Should().BeAssignableTo<BaseEntity<Guid>>();
    }

    [Fact]
    public void Model_DefaultValues_ShouldBeCorrect()
    {
        var model = new Model { Id = Guid.NewGuid() };

        model.Name.Should().BeEmpty();
        model.BrandId.Should().Be(Guid.Empty);
        model.Vehicles.Should().NotBeNull().And.BeEmpty();
        model.IsDeleted.Should().BeFalse();
        model.Brand.Should().BeNull();
    }
}
