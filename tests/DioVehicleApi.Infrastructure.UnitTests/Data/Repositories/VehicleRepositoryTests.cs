using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Infrastructure.Data;
using DioVehicleApi.Infrastructure.Data.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace DioVehicleApi.Infrastructure.UnitTests.Data.Repositories;

public class VehicleRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly VehicleRepository _repository;

    public VehicleRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options, new MockHttpContextAccessor());
        _repository = new VehicleRepository(_context);
    }

    [Fact]
    public async Task CreateAsync_ValidVehicle_ShouldAddToDatabase()
    {
        var modelId = Guid.NewGuid();
        var vehicle = new Vehicle { Id = Guid.NewGuid(), ModelId = modelId, Year = 2023, Color = "White", LicensePlate = "ABC-1234" };

        await _repository.CreateAsync(vehicle);
        await _repository.SaveChangesAsync();

        var result = await _context.Vehicles.FindAsync(vehicle.Id);
        result.Should().NotBeNull();
        result!.LicensePlate.Should().Be("ABC-1234");
    }

    [Fact]
    public async Task GetByIdAsync_ExistingVehicle_ShouldReturnVehicle()
    {
        var modelId = Guid.NewGuid();
        var vehicle = new Vehicle { Id = Guid.NewGuid(), ModelId = modelId, Year = 2023, Color = "Black", LicensePlate = "XYZ-5678" };
        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(vehicle.Id);

        result.Should().NotBeNull();
        result!.LicensePlate.Should().Be("XYZ-5678");
    }

    [Fact]
    public async Task Update_ExistingVehicle_ShouldModifyVehicle()
    {
        var modelId = Guid.NewGuid();
        var vehicle = new Vehicle { Id = Guid.NewGuid(), ModelId = modelId, Year = 2020, Color = "OldColor", LicensePlate = "OLD-1234" };
        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();

        vehicle.Color = "NewColor";
        _repository.Update(vehicle);
        await _repository.SaveChangesAsync();

        var result = await _context.Vehicles.FindAsync(vehicle.Id);
        result!.Color.Should().Be("NewColor");
    }

    [Fact]
    public async Task Remove_ExistingVehicle_ShouldSoftDelete()
    {
        var modelId = Guid.NewGuid();
        var vehicle = new Vehicle { Id = Guid.NewGuid(), ModelId = modelId, Year = 2023, Color = "Yellow", LicensePlate = "DEL-9999" };
        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();

        _repository.Remove(vehicle);
        await _repository.SaveChangesAsync();

        var result = await _context.Vehicles.FindAsync(vehicle.Id);
        result!.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task CountAsync_ShouldReturnCorrectCount()
    {
        var modelId = Guid.NewGuid();
        var vehicle1 = new Vehicle { Id = Guid.NewGuid(), ModelId = modelId, Year = 2023, Color = "Red", LicensePlate = "CNT-1111" };
        var vehicle2 = new Vehicle { Id = Guid.NewGuid(), ModelId = modelId, Year = 2023, Color = "Blue", LicensePlate = "CNT-2222" };
        _context.Vehicles.AddRange(vehicle1, vehicle2);
        await _context.SaveChangesAsync();

        var count = await _repository.CountAsync();

        count.Should().Be(2);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
