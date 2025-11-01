using DioVehicleApi.Application.Features.Vehicles.Commands.CreateVehicle;
using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Exceptions;
using DioVehicleApi.Domain.Interfaces.Repositories;
using FluentAssertions;
using Moq;

namespace DioVehicleApi.Application.UnitTests.Features.Vehicles.Commands.CreateVehicle;

public class CreateVehicleCommandHandlerTests
{
    private readonly Mock<IVehicleRepository> _mockVehicleRepository;
    private readonly Mock<IModelRepository> _mockModelRepository;
    private readonly CreateVehicleCommandHandler _handler;

    public CreateVehicleCommandHandlerTests()
    {
        _mockVehicleRepository = new Mock<IVehicleRepository>();
        _mockModelRepository = new Mock<IModelRepository>();
        _handler = new CreateVehicleCommandHandler(_mockVehicleRepository.Object, _mockModelRepository.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateVehicle()
    {
        var modelId = Guid.NewGuid();
        var model = new Model { Id = modelId, Name = "Corolla", BrandId = Guid.NewGuid(), IsDeleted = false };
        var command = new CreateVehicleCommand { ModelId = modelId, Year = 2023, Color = "White", LicensePlate = "ABC-1234" };

        _mockModelRepository.Setup(r => r.GetByIdAsync(modelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(model);
        _mockVehicleRepository.Setup(r => r.CreateAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Vehicle { Id = Guid.NewGuid(), ModelId = modelId, Year = 2023, Color = "White", LicensePlate = "ABC-1234" });
        _mockVehicleRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.ModelId.Should().Be(modelId);
        result.Year.Should().Be(2023);
        result.Color.Should().Be("White");
        result.LicensePlate.Should().Be("ABC-1234");
    }

    [Fact]
    public async Task Handle_ModelNotFound_ShouldThrowNotFoundException()
    {
        var modelId = Guid.NewGuid();
        var command = new CreateVehicleCommand { ModelId = modelId, Year = 2023, Color = "White", LicensePlate = "ABC-1234" };

        _mockModelRepository.Setup(r => r.GetByIdAsync(modelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Model?)null);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ModelIsDeleted_ShouldThrowDomainException()
    {
        var modelId = Guid.NewGuid();
        var model = new Model { Id = modelId, Name = "Corolla", BrandId = Guid.NewGuid(), IsDeleted = true };
        var command = new CreateVehicleCommand { ModelId = modelId, Year = 2023, Color = "White", LicensePlate = "ABC-1234" };

        _mockModelRepository.Setup(r => r.GetByIdAsync(modelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(model);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }
}
