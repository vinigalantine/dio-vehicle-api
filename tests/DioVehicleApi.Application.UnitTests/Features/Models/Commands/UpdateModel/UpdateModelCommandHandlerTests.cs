using DioVehicleApi.Application.Features.Models.Commands.UpdateModel;
using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Exceptions;
using DioVehicleApi.Domain.Interfaces.Repositories;
using FluentAssertions;
using Moq;

namespace DioVehicleApi.Application.UnitTests.Features.Models.Commands.UpdateModel;

public class UpdateModelCommandHandlerTests
{
    private readonly Mock<IModelRepository> _mockModelRepository;
    private readonly Mock<IBrandRepository> _mockBrandRepository;
    private readonly UpdateModelCommandHandler _handler;

    public UpdateModelCommandHandlerTests()
    {
        _mockModelRepository = new Mock<IModelRepository>();
        _mockBrandRepository = new Mock<IBrandRepository>();
        _handler = new UpdateModelCommandHandler(_mockModelRepository.Object, _mockBrandRepository.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldUpdateModel()
    {
        var modelId = Guid.NewGuid();
        var brandId = Guid.NewGuid();
        var model = new Model { Id = modelId, Name = "OldName", BrandId = brandId };
        var command = new UpdateModelCommand { Id = modelId, Name = "NewName", BrandId = brandId };

        _mockModelRepository.Setup(r => r.GetByIdAsync(modelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(model);
        _mockModelRepository.Setup(r => r.Update(It.IsAny<Model>()))
            .Returns(model);
        _mockModelRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be("NewName");
    }

    [Fact]
    public async Task Handle_ModelNotFound_ShouldThrowNotFoundException()
    {
        var modelId = Guid.NewGuid();
        var command = new UpdateModelCommand { Id = modelId, Name = "NewName", BrandId = Guid.NewGuid() };

        _mockModelRepository.Setup(r => r.GetByIdAsync(modelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Model?)null);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_BrandChanged_ShouldValidateNewBrand()
    {
        var modelId = Guid.NewGuid();
        var oldBrandId = Guid.NewGuid();
        var newBrandId = Guid.NewGuid();
        var model = new Model { Id = modelId, Name = "Corolla", BrandId = oldBrandId };
        var newBrand = new Brand { Id = newBrandId, Name = "Honda", IsDeleted = false };
        var command = new UpdateModelCommand { Id = modelId, Name = "Civic", BrandId = newBrandId };

        _mockModelRepository.Setup(r => r.GetByIdAsync(modelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(model);
        _mockBrandRepository.Setup(r => r.GetByIdAsync(newBrandId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(newBrand);
        _mockModelRepository.Setup(r => r.Update(It.IsAny<Model>()))
            .Returns(model);
        _mockModelRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        _mockBrandRepository.Verify(r => r.GetByIdAsync(newBrandId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
