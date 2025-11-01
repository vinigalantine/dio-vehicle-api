using DioVehicleApi.Application.Features.Brands.Commands.DeleteBrand;
using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Interfaces.Repositories;
using FluentAssertions;
using Moq;

namespace DioVehicleApi.Application.UnitTests.Features.Brands.Commands.DeleteBrand;

public class DeleteBrandCommandHandlerTests
{
    private readonly Mock<IBrandRepository> _mockRepository;
    private readonly DeleteBrandCommandHandler _handler;

    public DeleteBrandCommandHandlerTests()
    {
        _mockRepository = new Mock<IBrandRepository>();
        _handler = new DeleteBrandCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ExistingBrand_ShouldReturnTrue()
    {
        var brandId = Guid.NewGuid();
        var brand = new Brand { Id = brandId, Name = "Toyota" };
        var command = new DeleteBrandCommand { Id = brandId };

        _mockRepository.Setup(r => r.GetByIdAsync(brandId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(brand);
        _mockRepository.Setup(r => r.Remove(brand));
        _mockRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _mockRepository.Verify(r => r.GetByIdAsync(brandId, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.Remove(brand), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistentBrand_ShouldReturnFalse()
    {
        var brandId = Guid.NewGuid();
        var command = new DeleteBrandCommand { Id = brandId };

        _mockRepository.Setup(r => r.GetByIdAsync(brandId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Brand?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
        _mockRepository.Verify(r => r.GetByIdAsync(brandId, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.Remove(It.IsAny<Brand>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_SaveChangesReturnsZero_ShouldReturnFalse()
    {
        var brandId = Guid.NewGuid();
        var brand = new Brand { Id = brandId, Name = "Honda" };
        var command = new DeleteBrandCommand { Id = brandId };

        _mockRepository.Setup(r => r.GetByIdAsync(brandId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(brand);
        _mockRepository.Setup(r => r.Remove(brand));
        _mockRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WithCancellation_ShouldPassCancellationToken()
    {
        var brandId = Guid.NewGuid();
        var brand = new Brand { Id = brandId, Name = "Test" };
        var command = new DeleteBrandCommand { Id = brandId };
        var cancellationToken = new CancellationToken();

        _mockRepository.Setup(r => r.GetByIdAsync(brandId, cancellationToken))
            .ReturnsAsync(brand);
        _mockRepository.Setup(r => r.Remove(brand));
        _mockRepository.Setup(r => r.SaveChangesAsync(cancellationToken))
            .ReturnsAsync(1);

        await _handler.Handle(command, cancellationToken);

        _mockRepository.Verify(r => r.GetByIdAsync(brandId, cancellationToken), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidBrand_ShouldCallRemoveWithCorrectBrand()
    {
        var brandId = Guid.NewGuid();
        var brand = new Brand { Id = brandId, Name = "Ford" };
        var command = new DeleteBrandCommand { Id = brandId };
        Brand? removedBrand = null;

        _mockRepository.Setup(r => r.GetByIdAsync(brandId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(brand);
        _mockRepository.Setup(r => r.Remove(It.IsAny<Brand>()))
            .Callback<Brand>(b => removedBrand = b);
        _mockRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _handler.Handle(command, CancellationToken.None);

        removedBrand.Should().NotBeNull();
        removedBrand.Should().BeSameAs(brand);
        removedBrand!.Id.Should().Be(brandId);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(5)]
    public async Task Handle_DifferentRowsAffected_ShouldReturnTrue(int rowsAffected)
    {
        var brandId = Guid.NewGuid();
        var brand = new Brand { Id = brandId, Name = "Test" };
        var command = new DeleteBrandCommand { Id = brandId };

        _mockRepository.Setup(r => r.GetByIdAsync(brandId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(brand);
        _mockRepository.Setup(r => r.Remove(brand));
        _mockRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(rowsAffected);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
    }
}
