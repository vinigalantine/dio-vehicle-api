using DioVehicleApi.Application.Features.Vehicles.Commands.DeleteVehicle;
using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Interfaces.Repositories;
using FluentAssertions;
using Moq;

namespace DioVehicleApi.Application.UnitTests.Features.Vehicles.Commands.DeleteVehicle;

public class DeleteVehicleCommandHandlerTests
{
    private readonly Mock<IVehicleRepository> _mockRepository;
    private readonly DeleteVehicleCommandHandler _handler;

    public DeleteVehicleCommandHandlerTests()
    {
        _mockRepository = new Mock<IVehicleRepository>();
        _handler = new DeleteVehicleCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ExistingVehicle_ShouldReturnTrue()
    {
        var vehicleId = Guid.NewGuid();
        var vehicle = new Vehicle { Id = vehicleId, ModelId = Guid.NewGuid(), Year = 2023, Color = "White", LicensePlate = "ABC-1234" };
        var command = new DeleteVehicleCommand { Id = vehicleId };

        _mockRepository.Setup(r => r.GetByIdAsync(vehicleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicle);
        _mockRepository.Setup(r => r.Remove(vehicle));
        _mockRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_NonExistentVehicle_ShouldReturnFalse()
    {
        var vehicleId = Guid.NewGuid();
        var command = new DeleteVehicleCommand { Id = vehicleId };

        _mockRepository.Setup(r => r.GetByIdAsync(vehicleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Vehicle?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
