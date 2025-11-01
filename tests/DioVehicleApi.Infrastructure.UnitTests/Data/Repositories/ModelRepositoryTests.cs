using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Infrastructure.Data;
using DioVehicleApi.Infrastructure.Data.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace DioVehicleApi.Infrastructure.UnitTests.Data.Repositories;

public class ModelRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ModelRepository _repository;

    public ModelRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options, new MockHttpContextAccessor());
        _repository = new ModelRepository(_context);
    }

    [Fact]
    public async Task CreateAsync_ValidModel_ShouldAddToDatabase()
    {
        var brandId = Guid.NewGuid();
        var model = new Model { Id = Guid.NewGuid(), Name = "Corolla", BrandId = brandId };

        await _repository.CreateAsync(model);
        await _repository.SaveChangesAsync();

        var result = await _context.Models.FindAsync(model.Id);
        result.Should().NotBeNull();
        result!.Name.Should().Be("Corolla");
    }

    [Fact]
    public async Task GetByIdAsync_ExistingModel_ShouldReturnModel()
    {
        var brandId = Guid.NewGuid();
        var model = new Model { Id = Guid.NewGuid(), Name = "Civic", BrandId = brandId };
        _context.Models.Add(model);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(model.Id);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Civic");
    }

    [Fact]
    public async Task Update_ExistingModel_ShouldModifyModel()
    {
        var brandId = Guid.NewGuid();
        var model = new Model { Id = Guid.NewGuid(), Name = "OldName", BrandId = brandId };
        _context.Models.Add(model);
        await _context.SaveChangesAsync();

        model.Name = "NewName";
        _repository.Update(model);
        await _repository.SaveChangesAsync();

        var result = await _context.Models.FindAsync(model.Id);
        result!.Name.Should().Be("NewName");
    }

    [Fact]
    public async Task Remove_ExistingModel_ShouldSoftDelete()
    {
        var brandId = Guid.NewGuid();
        var model = new Model { Id = Guid.NewGuid(), Name = "ToDelete", BrandId = brandId };
        _context.Models.Add(model);
        await _context.SaveChangesAsync();

        _repository.Remove(model);
        await _repository.SaveChangesAsync();

        var result = await _context.Models.FindAsync(model.Id);
        result!.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task CountAsync_ShouldReturnCorrectCount()
    {
        var brandId = Guid.NewGuid();
        var model1 = new Model { Id = Guid.NewGuid(), Name = "Model1", BrandId = brandId };
        var model2 = new Model { Id = Guid.NewGuid(), Name = "Model2", BrandId = brandId };
        _context.Models.AddRange(model1, model2);
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
