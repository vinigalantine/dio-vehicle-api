using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Infrastructure.Data.Configurations;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DioVehicleApi.Infrastructure.UnitTests.Data.Configurations;

public class UserConfigurationTests
{
    private readonly ModelBuilder _modelBuilder;
    private readonly IMutableEntityType _entityType;

    public UserConfigurationTests()
    {
        _modelBuilder = new ModelBuilder();
        new UserConfiguration().Configure(_modelBuilder.Entity<User>());
        _entityType = _modelBuilder.Model.FindEntityType(typeof(User))!;
    }

    [Fact]
    public void UserConfiguration_ShouldMapToUsersTable()
    {
        _entityType.GetTableName().Should().Be("Users");
    }

    [Fact]
    public void UserConfiguration_UsernameProperty_ShouldBeRequired()
    {
        var usernameProperty = _entityType.FindProperty(nameof(User.Username));
        usernameProperty.Should().NotBeNull();
        usernameProperty!.IsNullable.Should().BeFalse();
    }

    [Fact]
    public void UserConfiguration_UsernameProperty_ShouldHaveMaxLength100()
    {
        var usernameProperty = _entityType.FindProperty(nameof(User.Username));
        usernameProperty!.GetMaxLength().Should().Be(100);
    }

    [Fact]
    public void UserConfiguration_ShouldHaveUniqueIndexOnUsername()
    {
        var index = _entityType.GetIndexes()
            .FirstOrDefault(i => i.Properties.Any(p => p.Name == nameof(User.Username)));
        
        index.Should().NotBeNull();
        index!.IsUnique.Should().BeTrue();
    }

    [Fact]
    public void UserConfiguration_PasswordHashProperty_ShouldBeRequired()
    {
        var passwordHashProperty = _entityType.FindProperty(nameof(User.PasswordHash));
        passwordHashProperty.Should().NotBeNull();
        passwordHashProperty!.IsNullable.Should().BeFalse();
    }

    [Fact]
    public void UserConfiguration_PasswordHashProperty_ShouldHaveMaxLength500()
    {
        var passwordHashProperty = _entityType.FindProperty(nameof(User.PasswordHash));
        passwordHashProperty!.GetMaxLength().Should().Be(512);
    }

    [Fact]
    public void UserConfiguration_IsAdminProperty_ShouldBeRequired()
    {
        var isAdminProperty = _entityType.FindProperty(nameof(User.IsAdmin));
        isAdminProperty.Should().NotBeNull();
        isAdminProperty!.IsNullable.Should().BeFalse();
    }
}
