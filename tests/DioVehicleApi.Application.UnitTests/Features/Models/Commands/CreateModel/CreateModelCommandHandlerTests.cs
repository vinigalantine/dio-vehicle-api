using DioVehicleApi.Application.Features.Models.Commands.CreateModel;
using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Exceptions;
using DioVehicleApi.Domain.Interfaces.Repositories;
using FluentAssertions;
using Moq;

namespace DioVehicleApi.Application.UnitTests.Features.Models.Commands.CreateModel;

public class CreateModelCommandHandlerTests
{
    private readonly Mock<IModelRepository> _mockModelRepository;
    private readonly Mock<IBrandRepository> _mockBrandRepository;
    private readonly CreateModelCommandHandler _handler;

    public CreateModelCommandHandlerTests()
    {
        _mockModelRepository = new Mock<IModelRepository>();
        _mockBrandRepository = new Mock<IBrandRepository>();
        _handler = new CreateModelCommandHandler(_mockModelRepository.Object, _mockBrandRepository.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateModel()
    {
        var brandId = Guid.NewGuid();
        var brand = new Brand { Id = brandId, Name = "Toyota", IsDeleted = false };
        var command = new CreateModelCommand { Name = "Corolla", BrandId = brandId };

        _mockBrandRepository.Setup(r => r.GetByIdAsync(brandId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(brand);
        _mockModelRepository.Setup(r => r.CreateAsync(It.IsAny<Model>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Model m, CancellationToken ct) => m);
        _mockModelRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be("Corolla");
        result.BrandId.Should().Be(brandId);
        _mockBrandRepository.Verify(r => r.GetByIdAsync(brandId, It.IsAny<CancellationToken>()), Times.Once);
        _mockModelRepository.Verify(r => r.CreateAsync(It.IsAny<Model>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockModelRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_BrandNotFound_ShouldThrowNotFoundException()
    {
        var brandId = Guid.NewGuid();
        var command = new CreateModelCommand { Name = "Corolla", BrandId = brandId };

        _mockBrandRepository.Setup(r => r.GetByIdAsync(brandId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Brand?)null);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_BrandIsDeleted_ShouldThrowDomainException()
    {
        var brandId = Guid.NewGuid();
        var brand = new Brand { Id = brandId, Name = "Toyota", IsDeleted = true };
        var command = new CreateModelCommand { Name = "Corolla", BrandId = brandId };

        _mockBrandRepository.Setup(r => r.GetByIdAsync(brandId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(brand);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }
}
