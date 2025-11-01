using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Infrastructure.Data;
using DioVehicleApi.Infrastructure.Data.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace DioVehicleApi.Infrastructure.UnitTests.Data.Repositories;

public class UserRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options, new MockHttpContextAccessor());
        _repository = new UserRepository(_context);
    }

    [Fact]
    public async Task CreateAsync_ValidUser_ShouldAddToDatabase()
    {
        var user = new User { Id = Guid.NewGuid(), Username = "testuser", PasswordHash = "hash123", IsAdmin = false };

        await _repository.CreateAsync(user);
        await _repository.SaveChangesAsync();

        var result = await _context.Users.FindAsync(user.Id);
        result.Should().NotBeNull();
        result!.Username.Should().Be("testuser");
    }

    [Fact]
    public async Task GetByIdAsync_ExistingUser_ShouldReturnUser()
    {
        var user = new User { Id = Guid.NewGuid(), Username = "admin", PasswordHash = "adminhash", IsAdmin = true };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(user.Id);

        result.Should().NotBeNull();
        result!.Username.Should().Be("admin");
        result.IsAdmin.Should().BeTrue();
    }

    [Fact]
    public async Task GetByUsernameAsync_ExistingUser_ShouldReturnUser()
    {
        var user = new User { Id = Guid.NewGuid(), Username = "uniqueuser", PasswordHash = "hash", IsAdmin = false };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByUsernameAsync("uniqueuser");

        result.Should().NotBeNull();
        result!.Username.Should().Be("uniqueuser");
    }

    [Fact]
    public async Task GetByUsernameAsync_NonExistentUser_ShouldReturnNull()
    {
        var result = await _repository.GetByUsernameAsync("nonexistent");

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUsers()
    {
        var user1 = new User { Id = Guid.NewGuid(), Username = "user1", PasswordHash = "hash1", IsAdmin = false };
        var user2 = new User { Id = Guid.NewGuid(), Username = "user2", PasswordHash = "hash2", IsAdmin = true };
        _context.Users.AddRange(user1, user2);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync(default);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Update_ExistingUser_ShouldModifyUser()
    {
        var user = new User { Id = Guid.NewGuid(), Username = "oldname", PasswordHash = "oldhash", IsAdmin = false };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        user.Username = "newname";
        user.IsAdmin = true;
        _repository.Update(user);
        await _repository.SaveChangesAsync();

        var result = await _context.Users.FindAsync(user.Id);
        result!.Username.Should().Be("newname");
        result.IsAdmin.Should().BeTrue();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
