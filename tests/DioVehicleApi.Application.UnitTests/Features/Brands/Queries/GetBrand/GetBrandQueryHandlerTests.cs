using DioVehicleApi.Application.Features.Brands.Queries.GetBrand;
using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Interfaces.Repositories;
using FluentAssertions;
using Moq;

namespace DioVehicleApi.Application.UnitTests.Features.Brands.Queries.GetBrand;

public class GetBrandQueryHandlerTests
{
    private readonly Mock<IBrandRepository> _mockRepository;
    private readonly GetBrandQueryHandler _handler;

    public GetBrandQueryHandlerTests()
    {
        _mockRepository = new Mock<IBrandRepository>();
        _handler = new GetBrandQueryHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ExistingBrand_ShouldReturnBrand()
    {
        var brandId = Guid.NewGuid();
        var brand = new Brand { Id = brandId, Name = "Toyota" };
        var query = new GetBrandQuery { Id = brandId };

        _mockRepository.Setup(r => r.GetByIdAsync(brandId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(brand);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(brandId);
        result.Name.Should().Be("Toyota");
        _mockRepository.Verify(r => r.GetByIdAsync(brandId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistentBrand_ShouldReturnNull()
    {
        var brandId = Guid.NewGuid();
        var query = new GetBrandQuery { Id = brandId };

        _mockRepository.Setup(r => r.GetByIdAsync(brandId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Brand?)null);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeNull();
        _mockRepository.Verify(r => r.GetByIdAsync(brandId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithCancellation_ShouldPassCancellationToken()
    {
        var brandId = Guid.NewGuid();
        var brand = new Brand { Id = brandId, Name = "Honda" };
        var query = new GetBrandQuery { Id = brandId };
        var cancellationToken = new CancellationToken();

        _mockRepository.Setup(r => r.GetByIdAsync(brandId, cancellationToken))
            .ReturnsAsync(brand);

        await _handler.Handle(query, cancellationToken);

        _mockRepository.Verify(r => r.GetByIdAsync(brandId, cancellationToken), Times.Once);
    }

    [Theory]
    [InlineData("Toyota")]
    [InlineData("Honda")]
    [InlineData("Ford")]
    public async Task Handle_DifferentBrands_ShouldReturnCorrectBrand(string brandName)
    {
        var brandId = Guid.NewGuid();
        var brand = new Brand { Id = brandId, Name = brandName };
        var query = new GetBrandQuery { Id = brandId };

        _mockRepository.Setup(r => r.GetByIdAsync(brandId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(brand);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Name.Should().Be(brandName);
    }

    [Fact]
    public async Task Handle_ShouldOnlyCallRepositoryOnce()
    {
        var brandId = Guid.NewGuid();
        var brand = new Brand { Id = brandId, Name = "Test" };
        var query = new GetBrandQuery { Id = brandId };

        _mockRepository.Setup(r => r.GetByIdAsync(brandId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(brand);

        await _handler.Handle(query, CancellationToken.None);

        _mockRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnedBrand_ShouldHaveCorrectProperties()
    {
        var brandId = Guid.NewGuid();
        var brand = new Brand
        {
            Id = brandId,
            Name = "BMW",
            IsDeleted = false
        };
        var query = new GetBrandQuery { Id = brandId };

        _mockRepository.Setup(r => r.GetByIdAsync(brandId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(brand);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(brandId);
        result.Name.Should().Be("BMW");
        result.IsDeleted.Should().BeFalse();
    }
}
