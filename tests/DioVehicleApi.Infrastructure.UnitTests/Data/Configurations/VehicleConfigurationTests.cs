using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Infrastructure.Data.Configurations;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DioVehicleApi.Infrastructure.UnitTests.Data.Configurations;

public class VehicleConfigurationTests
{
    private readonly ModelBuilder _modelBuilder;
    private readonly IMutableEntityType _entityType;

    public VehicleConfigurationTests()
    {
        _modelBuilder = new ModelBuilder();
        new VehicleConfiguration().Configure(_modelBuilder.Entity<Vehicle>());
        _entityType = _modelBuilder.Model.FindEntityType(typeof(Vehicle))!;
    }

    [Fact]
    public void VehicleConfiguration_ShouldMapToVehiclesTable()
    {
        _entityType.GetTableName().Should().Be("Vehicles");
    }

    [Fact]
    public void VehicleConfiguration_LicensePlateProperty_ShouldBeRequired()
    {
        var licensePlateProperty = _entityType.FindProperty(nameof(Vehicle.LicensePlate));
        licensePlateProperty.Should().NotBeNull();
        licensePlateProperty!.IsNullable.Should().BeFalse();
    }

    [Fact]
    public void VehicleConfiguration_LicensePlateProperty_ShouldHaveMaxLength20()
    {
        var licensePlateProperty = _entityType.FindProperty(nameof(Vehicle.LicensePlate));
        licensePlateProperty!.GetMaxLength().Should().Be(20);
    }

    [Fact]
    public void VehicleConfiguration_ShouldHaveUniqueIndexOnLicensePlate()
    {
        var index = _entityType.GetIndexes()
            .FirstOrDefault(i => i.Properties.Any(p => p.Name == nameof(Vehicle.LicensePlate)));
        
        index.Should().NotBeNull();
        index!.IsUnique.Should().BeTrue();
    }

    [Fact]
    public void VehicleConfiguration_ColorProperty_ShouldBeRequired()
    {
        var colorProperty = _entityType.FindProperty(nameof(Vehicle.Color));
        colorProperty.Should().NotBeNull();
        colorProperty!.IsNullable.Should().BeFalse();
    }

    [Fact]
    public void VehicleConfiguration_ColorProperty_ShouldHaveMaxLength50()
    {
        var colorProperty = _entityType.FindProperty(nameof(Vehicle.Color));
        colorProperty!.GetMaxLength().Should().Be(50);
    }

    [Fact]
    public void VehicleConfiguration_ShouldHaveRelationshipWithModel()
    {
        var navigation = _entityType.FindNavigation(nameof(Vehicle.Model));
        navigation.Should().NotBeNull();
    }
}
