using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Interfaces.Repositories;
using DioVehicleApi.Infrastructure.Data;
using DioVehicleApi.Infrastructure.Data.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace DioVehicleApi.Infrastructure.UnitTests.Data.Repositories;

public class BrandRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly IBrandRepository _repository;

    public BrandRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options, new MockHttpContextAccessor());
        _repository = new BrandRepository(_context);
    }

    [Fact]
    public async Task CreateAsync_ValidBrand_ShouldAddToDatabase()
    {
        var brand = new Brand
        {
            Id = Guid.NewGuid(),
            Name = "Toyota"
        };

        var result = await _repository.CreateAsync(brand);
        await _repository.SaveChangesAsync();

        result.Should().NotBeNull();
        result.Id.Should().Be(brand.Id);
        result.Name.Should().Be("Toyota");

        var savedBrand = await _context.Brands.FindAsync(brand.Id);
        savedBrand.Should().NotBeNull();
        savedBrand!.Name.Should().Be("Toyota");
    }

    [Fact]
    public async Task GetByIdAsync_ExistingBrand_ShouldReturnBrand()
    {
        var brandId = Guid.NewGuid();
        var brand = new Brand { Id = brandId, Name = "Honda" };
        _context.Brands.Add(brand);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(brandId);

        result.Should().NotBeNull();
        result!.Id.Should().Be(brandId);
        result.Name.Should().Be("Honda");
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingBrand_ShouldReturnNull()
    {
        var nonExistingId = Guid.NewGuid();

        var result = await _repository.GetByIdAsync(nonExistingId);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_WithMultipleBrands_ShouldReturnAllNonDeleted()
    {
        var brand1 = new Brand { Id = Guid.NewGuid(), Name = "Ford" };
        var brand2 = new Brand { Id = Guid.NewGuid(), Name = "BMW" };
        var brand3 = new Brand { Id = Guid.NewGuid(), Name = "Audi", IsDeleted = true };

        _context.Brands.AddRange(brand1, brand2, brand3);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync(default);

        result.Should().HaveCount(2);
        result.Should().Contain(b => b.Name == "Ford");
        result.Should().Contain(b => b.Name == "BMW");
        result.Should().NotContain(b => b.Name == "Audi");
    }

    [Fact]
    public async Task Update_ExistingBrand_ShouldModifyBrand()
    {
        var brandId = Guid.NewGuid();
        var brand = new Brand { Id = brandId, Name = "Mercedes" };
        _context.Brands.Add(brand);
        await _context.SaveChangesAsync();

        brand.Name = "Mercedes-Benz";
        _repository.Update(brand);
        await _repository.SaveChangesAsync();

        var updated = await _context.Brands.FindAsync(brandId);
        updated.Should().NotBeNull();
        updated!.Name.Should().Be("Mercedes-Benz");
    }

    [Fact]
    public async Task CountAsync_WithFilter_ShouldReturnCorrectCount()
    {
        _context.Brands.AddRange(
            new Brand { Id = Guid.NewGuid(), Name = "Toyota" },
            new Brand { Id = Guid.NewGuid(), Name = "Honda" },
            new Brand { Id = Guid.NewGuid(), Name = "Ford", IsDeleted = true }
        );
        await _context.SaveChangesAsync();

        var count = await _repository.CountAsync();

        count.Should().Be(2);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldPersistChanges()
    {
        var brand = new Brand { Id = Guid.NewGuid(), Name = "Nissan" };

        await _repository.CreateAsync(brand);
        var changes = await _repository.SaveChangesAsync();

        changes.Should().Be(1);
        var saved = await _context.Brands.FindAsync(brand.Id);
        saved.Should().NotBeNull();
    }

    [Fact]
    public async Task ExistsByNameAsync_ExistingBrand_ShouldReturnTrue()
    {
        var brand = new Brand { Id = Guid.NewGuid(), Name = "Toyota" };
        _context.Brands.Add(brand);
        await _context.SaveChangesAsync();

        var result = await _repository.ExistsByNameAsync("Toyota");

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByNameAsync_NonExistingBrand_ShouldReturnFalse()
    {
        var brand = new Brand { Id = Guid.NewGuid(), Name = "Toyota" };
        _context.Brands.Add(brand);
        await _context.SaveChangesAsync();

        var result = await _repository.ExistsByNameAsync("Honda");

        result.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsByNameAsync_SoftDeletedBrand_ShouldReturnFalse()
    {
        var brand = new Brand { Id = Guid.NewGuid(), Name = "Toyota", IsDeleted = true };
        _context.Brands.Add(brand);
        await _context.SaveChangesAsync();

        var result = await _repository.ExistsByNameAsync("Toyota");

        result.Should().BeFalse();
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}
