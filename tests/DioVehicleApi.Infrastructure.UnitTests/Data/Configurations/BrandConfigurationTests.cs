using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Infrastructure.Data.Configurations;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace DioVehicleApi.Infrastructure.UnitTests.Data.Configurations;

public class BrandConfigurationTests
{
    private ModelBuilder CreateModelBuilder()
    {
        var conventionSet = new ConventionSet();
        var modelBuilder = new ModelBuilder(conventionSet);
        return modelBuilder;
    }

    [Fact]
    public void Configure_ShouldSetTableName()
    {
        var modelBuilder = CreateModelBuilder();
        var configuration = new BrandConfiguration();

        configuration.Configure(modelBuilder.Entity<Brand>());
        var model = modelBuilder.FinalizeModel();
        var entityType = model.FindEntityType(typeof(Brand));

        entityType.Should().NotBeNull();
        entityType!.GetTableName().Should().Be("Brands");
    }

    [Fact]
    public void Configure_NameProperty_ShouldBeRequired()
    {
        var modelBuilder = CreateModelBuilder();
        var configuration = new BrandConfiguration();

        configuration.Configure(modelBuilder.Entity<Brand>());
        var model = modelBuilder.FinalizeModel();
        var entityType = model.FindEntityType(typeof(Brand));
        var nameProperty = entityType!.FindProperty(nameof(Brand.Name));

        nameProperty.Should().NotBeNull();
        nameProperty!.IsNullable.Should().BeFalse();
    }

    [Fact]
    public void Configure_NameProperty_ShouldHaveMaxLength100()
    {
        var modelBuilder = CreateModelBuilder();
        var configuration = new BrandConfiguration();

        configuration.Configure(modelBuilder.Entity<Brand>());
        var model = modelBuilder.FinalizeModel();
        var entityType = model.FindEntityType(typeof(Brand));
        var nameProperty = entityType!.FindProperty(nameof(Brand.Name));

        nameProperty.Should().NotBeNull();
        nameProperty!.GetMaxLength().Should().Be(100);
    }

    [Fact]
    public void Configure_ShouldHaveUniqueIndexOnName()
    {
        var modelBuilder = CreateModelBuilder();
        var configuration = new BrandConfiguration();

        configuration.Configure(modelBuilder.Entity<Brand>());
        var model = modelBuilder.FinalizeModel();
        var entityType = model.FindEntityType(typeof(Brand));
        var index = entityType!.GetIndexes()
            .FirstOrDefault(i => i.Properties.Any(p => p.Name == nameof(Brand.Name)));

        index.Should().NotBeNull();
        index!.IsUnique.Should().BeTrue();
        index.GetDatabaseName().Should().Be("IX_Brands_Name");
    }

    [Fact]
    public void Configure_ShouldHaveOneToManyRelationshipWithModels()
    {
        var modelBuilder = CreateModelBuilder();
        var brandConfiguration = new BrandConfiguration();
        var modelConfiguration = new ModelConfiguration();

        brandConfiguration.Configure(modelBuilder.Entity<Brand>());
        modelConfiguration.Configure(modelBuilder.Entity<Model>());
        var model = modelBuilder.FinalizeModel();
        var entityType = model.FindEntityType(typeof(Brand));
        var navigation = entityType!.FindNavigation(nameof(Brand.Models));

        navigation.Should().NotBeNull();
        navigation!.IsCollection.Should().BeTrue();
        navigation.ForeignKey.DeleteBehavior.Should().Be(DeleteBehavior.Restrict);
    }

    [Fact]
    public void Configure_ShouldInheritBaseEntityConfiguration()
    {
        var modelBuilder = CreateModelBuilder();
        var configuration = new BrandConfiguration();

        configuration.Configure(modelBuilder.Entity<Brand>());
        var model = modelBuilder.FinalizeModel();
        var entityType = model.FindEntityType(typeof(Brand));

        entityType!.FindProperty("Id").Should().NotBeNull();
        entityType.FindProperty("CreatedAt").Should().NotBeNull();
        entityType.FindProperty("CreatedBy").Should().NotBeNull();
        entityType.FindProperty("UpdatedAt").Should().NotBeNull();
        entityType.FindProperty("UpdatedBy").Should().NotBeNull();
    }

    [Fact]
    public void Configure_CreatedByProperty_ShouldHaveMaxLength256()
    {
        var modelBuilder = CreateModelBuilder();
        var configuration = new BrandConfiguration();

        configuration.Configure(modelBuilder.Entity<Brand>());
        var model = modelBuilder.FinalizeModel();
        var entityType = model.FindEntityType(typeof(Brand));
        var createdByProperty = entityType!.FindProperty("CreatedBy");

        createdByProperty.Should().NotBeNull();
        createdByProperty!.GetMaxLength().Should().Be(256);
    }

    [Fact]
    public void Configure_SoftDeleteProperties_ShouldBeConfigured()
    {
        var modelBuilder = CreateModelBuilder();
        var configuration = new BrandConfiguration();

        configuration.Configure(modelBuilder.Entity<Brand>());
        var model = modelBuilder.FinalizeModel();
        var entityType = model.FindEntityType(typeof(Brand));

        entityType!.FindProperty(nameof(Brand.IsDeleted)).Should().NotBeNull();
        entityType.FindProperty(nameof(Brand.DeletedAt)).Should().NotBeNull();
        entityType.FindProperty(nameof(Brand.DeletedBy)).Should().NotBeNull();
    }
}
