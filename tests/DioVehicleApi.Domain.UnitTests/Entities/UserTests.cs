using DioVehicleApi.Domain.Entities;
using FluentAssertions;

namespace DioVehicleApi.Domain.UnitTests.Entities;

public class UserTests
{
    [Fact]
    public void CreateUser_WithValidProperties_ShouldSetCorrectly()
    {
        var id = Guid.NewGuid();
        var username = "testuser";
        var passwordHash = "hashedpassword123";
        var isAdmin = false;

        var user = new User
        {
            Id = id,
            Username = username,
            PasswordHash = passwordHash,
            IsAdmin = isAdmin
        };

        user.Should().NotBeNull();
        user.Id.Should().Be(id);
        user.Username.Should().Be(username);
        user.PasswordHash.Should().Be(passwordHash);
        user.IsAdmin.Should().Be(isAdmin);
    }

    [Fact]
    public void CreateUser_AsAdmin_ShouldSetIsAdminTrue()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "admin",
            PasswordHash = "adminHash",
            IsAdmin = true
        };

        user.IsAdmin.Should().BeTrue();
    }

    [Fact]
    public void CreateUser_AsNormalUser_ShouldSetIsAdminFalse()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "normaluser",
            PasswordHash = "userHash",
            IsAdmin = false
        };

        user.IsAdmin.Should().BeFalse();
    }

    [Theory]
    [InlineData("admin")]
    [InlineData("user1")]
    [InlineData("john.doe")]
    [InlineData("test@example.com")]
    public void User_WithDifferentUsernames_ShouldStoreCorrectly(string username)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            PasswordHash = "hash",
            IsAdmin = false
        };

        user.Username.Should().Be(username);
    }

    [Fact]
    public void User_PasswordHash_ShouldBeStoredSecurely()
    {
        var passwordHash = "5f4dcc3b5aa765d61d8327deb882cf99";

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            PasswordHash = passwordHash,
            IsAdmin = false
        };

        user.PasswordHash.Should().Be(passwordHash);
        user.PasswordHash.Should().NotBe("password");
    }

    [Fact]
    public void User_ShouldInheritFromBaseEntity()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            PasswordHash = "hash",
            IsAdmin = false
        };

        user.Should().BeAssignableTo<BaseEntity<Guid>>();
    }

    [Fact]
    public void User_DefaultValues_ShouldBeCorrect()
    {
        var user = new User { Id = Guid.NewGuid() };

        user.Username.Should().BeEmpty();
        user.PasswordHash.Should().BeEmpty();
        user.IsAdmin.Should().BeFalse();
    }

    [Fact]
    public void User_IsAdminFlag_ShouldDistinguishAdminFromNormalUsers()
    {
        var admin = new User
        {
            Id = Guid.NewGuid(),
            Username = "admin",
            PasswordHash = "hash1",
            IsAdmin = true
        };

        var normalUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "user",
            PasswordHash = "hash2",
            IsAdmin = false
        };

        admin.IsAdmin.Should().BeTrue();
        normalUser.IsAdmin.Should().BeFalse();
        admin.IsAdmin.Should().NotBe(normalUser.IsAdmin);
    }
}
