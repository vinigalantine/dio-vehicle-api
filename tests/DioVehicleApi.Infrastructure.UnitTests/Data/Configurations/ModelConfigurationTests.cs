using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Infrastructure.Data.Configurations;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DioVehicleApi.Infrastructure.UnitTests.Data.Configurations;

public class ModelConfigurationTests
{
    private readonly ModelBuilder _modelBuilder;
    private readonly IMutableEntityType _entityType;

    public ModelConfigurationTests()
    {
        _modelBuilder = new ModelBuilder();
        new ModelConfiguration().Configure(_modelBuilder.Entity<Model>());
        _entityType = _modelBuilder.Model.FindEntityType(typeof(Model))!;
    }

    [Fact]
    public void ModelConfiguration_ShouldMapToModelsTable()
    {
        _entityType.GetTableName().Should().Be("Models");
    }

    [Fact]
    public void ModelConfiguration_NameProperty_ShouldBeRequired()
    {
        var nameProperty = _entityType.FindProperty(nameof(Model.Name));
        nameProperty.Should().NotBeNull();
        nameProperty!.IsNullable.Should().BeFalse();
    }

    [Fact]
    public void ModelConfiguration_NameProperty_ShouldHaveMaxLength100()
    {
        var nameProperty = _entityType.FindProperty(nameof(Model.Name));
        nameProperty!.GetMaxLength().Should().Be(100);
    }

    [Fact]
    public void ModelConfiguration_ShouldHaveUniqueIndexOnBrandIdAndName()
    {
        var index = _entityType.GetIndexes()
            .FirstOrDefault(i => i.Properties.Any(p => p.Name == nameof(Model.BrandId)) && 
                                i.Properties.Any(p => p.Name == nameof(Model.Name)));
        
        index.Should().NotBeNull();
        index!.IsUnique.Should().BeTrue();
    }

    [Fact]
    public void ModelConfiguration_ShouldHaveRelationshipWithBrand()
    {
        var navigation = _entityType.FindNavigation(nameof(Model.Brand));
        navigation.Should().NotBeNull();
    }

    [Fact]
    public void ModelConfiguration_ShouldHaveCollectionOfVehicles()
    {
        var navigation = _entityType.FindNavigation(nameof(Model.Vehicles));
        navigation.Should().NotBeNull();
        navigation!.IsCollection.Should().BeTrue();
    }
}
