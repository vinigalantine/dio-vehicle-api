using DioVehicleApi.Application.Features.Brands.Commands.UpdateBrand;
using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Exceptions;
using DioVehicleApi.Domain.Interfaces.Repositories;
using FluentAssertions;
using Moq;

namespace DioVehicleApi.Application.UnitTests.Features.Brands.Commands.UpdateBrand;

public class UpdateBrandCommandHandlerTests
{
    private readonly Mock<IBrandRepository> _mockRepository;
    private readonly UpdateBrandCommandHandler _handler;

    public UpdateBrandCommandHandlerTests()
    {
        _mockRepository = new Mock<IBrandRepository>();
        _handler = new UpdateBrandCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldUpdateBrand()
    {
        var brandId = Guid.NewGuid();
        var existingBrand = new Brand { Id = brandId, Name = "OldName" };
        var command = new UpdateBrandCommand { Id = brandId, Name = "NewName" };

        _mockRepository.Setup(r => r.GetByIdAsync(brandId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBrand);
        _mockRepository.Setup(r => r.Update(It.IsAny<Brand>()))
            .Returns(existingBrand);
        _mockRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be("NewName");
        _mockRepository.Verify(r => r.GetByIdAsync(brandId, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.Update(It.Is<Brand>(b => b.Name == "NewName")), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistentBrand_ShouldThrowNotFoundException()
    {
        var brandId = Guid.NewGuid();
        var command = new UpdateBrandCommand { Id = brandId, Name = "NewName" };

        _mockRepository.Setup(r => r.GetByIdAsync(brandId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Brand?)null);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Brand with ID '{brandId}' was not found");
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldOnlyUpdateName()
    {
        var brandId = Guid.NewGuid();
        var existingBrand = new Brand { Id = brandId, Name = "Original" };
        var command = new UpdateBrandCommand { Id = brandId, Name = "Updated" };
        
        Brand? capturedBrand = null;
        _mockRepository.Setup(r => r.GetByIdAsync(brandId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBrand);
        _mockRepository.Setup(r => r.Update(It.IsAny<Brand>()))
            .Callback<Brand>(b => capturedBrand = b)
            .Returns(existingBrand);
        _mockRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _handler.Handle(command, CancellationToken.None);

        capturedBrand.Should().NotBeNull();
        capturedBrand!.Id.Should().Be(brandId); // ID unchanged
        capturedBrand.Name.Should().Be("Updated"); // Name changed
    }

    [Fact]
    public async Task Handle_WithCancellation_ShouldPassCancellationToken()
    {
        var brandId = Guid.NewGuid();
        var existingBrand = new Brand { Id = brandId, Name = "Test" };
        var command = new UpdateBrandCommand { Id = brandId, Name = "Updated" };
        var cancellationToken = new CancellationToken();

        _mockRepository.Setup(r => r.GetByIdAsync(brandId, cancellationToken))
            .ReturnsAsync(existingBrand);
        _mockRepository.Setup(r => r.Update(It.IsAny<Brand>()))
            .Returns(existingBrand);
        _mockRepository.Setup(r => r.SaveChangesAsync(cancellationToken))
            .ReturnsAsync(1);

        await _handler.Handle(command, cancellationToken);

        _mockRepository.Verify(r => r.GetByIdAsync(brandId, cancellationToken), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryReturnsUpdatedEntity_ShouldReturnSameEntity()
    {
        var brandId = Guid.NewGuid();
        var existingBrand = new Brand { Id = brandId, Name = "Original" };
        var command = new UpdateBrandCommand { Id = brandId, Name = "Modified" };

        _mockRepository.Setup(r => r.GetByIdAsync(brandId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBrand);
        _mockRepository.Setup(r => r.Update(It.IsAny<Brand>()))
            .Returns(existingBrand);
        _mockRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeSameAs(existingBrand);
        result.Id.Should().Be(brandId);
    }

    [Theory]
    [InlineData("Toyota")]
    [InlineData("Honda")]
    [InlineData("Ford")]
    public async Task Handle_DifferentNames_ShouldUpdateCorrectly(string newName)
    {
        var brandId = Guid.NewGuid();
        var existingBrand = new Brand { Id = brandId, Name = "OldName" };
        var command = new UpdateBrandCommand { Id = brandId, Name = newName };

        _mockRepository.Setup(r => r.GetByIdAsync(brandId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBrand);
        _mockRepository.Setup(r => r.Update(It.IsAny<Brand>()))
            .Returns(existingBrand);
        _mockRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Name.Should().Be(newName);
    }

    [Fact]
    public async Task Handle_SaveChangesFails_ShouldStillReturnUpdatedBrand()
    {
        var brandId = Guid.NewGuid();
        var existingBrand = new Brand { Id = brandId, Name = "Test" };
        var command = new UpdateBrandCommand { Id = brandId, Name = "Updated" };

        _mockRepository.Setup(r => r.GetByIdAsync(brandId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBrand);
        _mockRepository.Setup(r => r.Update(It.IsAny<Brand>()))
            .Returns(existingBrand);
        _mockRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be("Updated");
    }
}
