using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Interfaces;
using FluentAssertions;

namespace DioVehicleApi.Domain.UnitTests.Entities;

public class BrandTests
{
    [Fact]
    public void CreateBrand_WithValidName_ShouldSetProperties()
    {
        var id = Guid.NewGuid();
        var name = "Toyota";

        var brand = new Brand
        {
            Id = id,
            Name = name
        };

        brand.Should().NotBeNull();
        brand.Id.Should().Be(id);
        brand.Name.Should().Be(name);
        brand.Models.Should().NotBeNull().And.BeEmpty();
        brand.IsDeleted.Should().BeFalse();
        brand.DeletedAt.Should().BeNull();
        brand.DeletedBy.Should().BeNull();
    }

    [Fact]
    public void CreateBrand_WithoutSettingProperties_ShouldHaveDefaults()
    {
        var brand = new Brand { Id = Guid.NewGuid() };

        brand.Should().NotBeNull();
        brand.Id.Should().NotBeEmpty();
        brand.Name.Should().BeEmpty();
        brand.IsDeleted.Should().BeFalse();
        brand.Models.Should().NotBeNull();
    }

    [Fact]
    public void Brand_ShouldAllowAddingModels()
    {
        var brand = new Brand { Id = Guid.NewGuid(), Name = "Honda" };
        var modelId = Guid.NewGuid();
        var model = new Model 
        { 
            Id = modelId,
            Name = "Civic", 
            BrandId = brand.Id 
        };

        brand.Models.Add(model);

        brand.Models.Should().HaveCount(1);
        brand.Models.Should().Contain(model);
        brand.Models.First().BrandId.Should().Be(brand.Id);
    }

    [Theory]
    [InlineData("Toyota")]
    [InlineData("Ford")]
    [InlineData("BMW")]
    [InlineData("Mercedes-Benz")]
    public void Brand_WithDifferentNames_ShouldStoreCorrectly(string name)
    {
        var brand = new Brand { Id = Guid.NewGuid(), Name = name };

        brand.Name.Should().Be(name);
        brand.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Brand_WhenSoftDeleted_ShouldSetAllDeleteProperties()
    {
        var brand = new Brand { Id = Guid.NewGuid(), Name = "Mercedes" };
        var deletedAt = DateTime.UtcNow;
        var deletedBy = "TestUser";

        brand.IsDeleted = true;
        brand.DeletedAt = deletedAt;
        brand.DeletedBy = deletedBy;

        brand.IsDeleted.Should().BeTrue();
        brand.DeletedAt.Should().Be(deletedAt);
        brand.DeletedBy.Should().Be(deletedBy);
    }

    [Fact]
    public void Brand_ShouldImplementISoftDeletable()
    {
        var brand = new Brand { Id = Guid.NewGuid(), Name = "Audi" };

        brand.Should().BeAssignableTo<ISoftDeletable>();
    }

    [Fact]
    public void Brand_ShouldInheritFromBaseEntity()
    {
        var brand = new Brand { Id = Guid.NewGuid(), Name = "Volvo" };

        brand.Should().BeAssignableTo<BaseEntity<Guid>>();
    }
}

