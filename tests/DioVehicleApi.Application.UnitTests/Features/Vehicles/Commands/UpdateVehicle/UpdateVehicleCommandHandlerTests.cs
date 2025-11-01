using DioVehicleApi.Application.Features.Vehicles.Commands.UpdateVehicle;
using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Exceptions;
using DioVehicleApi.Domain.Interfaces.Repositories;
using FluentAssertions;
using Moq;

namespace DioVehicleApi.Application.UnitTests.Features.Vehicles.Commands.UpdateVehicle;

public class UpdateVehicleCommandHandlerTests
{
    private readonly Mock<IVehicleRepository> _mockVehicleRepository;
    private readonly Mock<IModelRepository> _mockModelRepository;
    private readonly UpdateVehicleCommandHandler _handler;

    public UpdateVehicleCommandHandlerTests()
    {
        _mockVehicleRepository = new Mock<IVehicleRepository>();
        _mockModelRepository = new Mock<IModelRepository>();
        _handler = new UpdateVehicleCommandHandler(_mockVehicleRepository.Object, _mockModelRepository.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldUpdateVehicle()
    {
        var vehicleId = Guid.NewGuid();
        var modelId = Guid.NewGuid();
        var vehicle = new Vehicle { Id = vehicleId, ModelId = modelId, Year = 2020, Color = "Red", LicensePlate = "OLD-1234" };
        var command = new UpdateVehicleCommand { Id = vehicleId, ModelId = modelId, Year = 2023, Color = "Blue", LicensePlate = "NEW-5678" };

        _mockVehicleRepository.Setup(r => r.GetByIdAsync(vehicleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicle);
        _mockVehicleRepository.Setup(r => r.Update(It.IsAny<Vehicle>()))
            .Returns(vehicle);
        _mockVehicleRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Year.Should().Be(2023);
        result.Color.Should().Be("Blue");
        result.LicensePlate.Should().Be("NEW-5678");
    }

    [Fact]
    public async Task Handle_VehicleNotFound_ShouldThrowNotFoundException()
    {
        var vehicleId = Guid.NewGuid();
        var command = new UpdateVehicleCommand { Id = vehicleId, ModelId = Guid.NewGuid(), Year = 2023, Color = "Blue", LicensePlate = "ABC-1234" };

        _mockVehicleRepository.Setup(r => r.GetByIdAsync(vehicleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Vehicle?)null);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ModelChanged_ShouldValidateNewModel()
    {
        var vehicleId = Guid.NewGuid();
        var oldModelId = Guid.NewGuid();
        var newModelId = Guid.NewGuid();
        var vehicle = new Vehicle { Id = vehicleId, ModelId = oldModelId, Year = 2020, Color = "Red", LicensePlate = "ABC-1234" };
        var newModel = new Model { Id = newModelId, Name = "Civic", BrandId = Guid.NewGuid(), IsDeleted = false };
        var command = new UpdateVehicleCommand { Id = vehicleId, ModelId = newModelId, Year = 2023, Color = "Blue", LicensePlate = "XYZ-5678" };

        _mockVehicleRepository.Setup(r => r.GetByIdAsync(vehicleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicle);
        _mockModelRepository.Setup(r => r.GetByIdAsync(newModelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(newModel);
        _mockVehicleRepository.Setup(r => r.Update(It.IsAny<Vehicle>()))
            .Returns(vehicle);
        _mockVehicleRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        _mockModelRepository.Verify(r => r.GetByIdAsync(newModelId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
