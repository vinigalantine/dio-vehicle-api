using DioVehicleApi.Application.Features.Models.Commands.DeleteModel;
using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Interfaces.Repositories;
using FluentAssertions;
using Moq;

namespace DioVehicleApi.Application.UnitTests.Features.Models.Commands.DeleteModel;

public class DeleteModelCommandHandlerTests
{
    private readonly Mock<IModelRepository> _mockRepository;
    private readonly DeleteModelCommandHandler _handler;

    public DeleteModelCommandHandlerTests()
    {
        _mockRepository = new Mock<IModelRepository>();
        _handler = new DeleteModelCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ExistingModel_ShouldReturnTrue()
    {
        var modelId = Guid.NewGuid();
        var model = new Model { Id = modelId, Name = "Corolla", BrandId = Guid.NewGuid() };
        var command = new DeleteModelCommand { Id = modelId };

        _mockRepository.Setup(r => r.GetByIdAsync(modelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(model);
        _mockRepository.Setup(r => r.Remove(model));
        _mockRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_NonExistentModel_ShouldReturnFalse()
    {
        var modelId = Guid.NewGuid();
        var command = new DeleteModelCommand { Id = modelId };

        _mockRepository.Setup(r => r.GetByIdAsync(modelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Model?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
