using DioVehicleApi.Application.Features.Vehicles.Queries.GetVehicle;
using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Interfaces.Repositories;
using FluentAssertions;
using Moq;

namespace DioVehicleApi.Application.UnitTests.Features.Vehicles.Queries.GetVehicle;

public class GetVehicleQueryHandlerTests
{
    private readonly Mock<IVehicleRepository> _mockRepository;
    private readonly GetVehicleQueryHandler _handler;

    public GetVehicleQueryHandlerTests()
    {
        _mockRepository = new Mock<IVehicleRepository>();
        _handler = new GetVehicleQueryHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ExistingVehicle_ShouldReturnVehicle()
    {
        var vehicleId = Guid.NewGuid();
        var vehicle = new Vehicle { Id = vehicleId, ModelId = Guid.NewGuid(), Year = 2023, Color = "White", LicensePlate = "ABC-1234" };
        var query = new GetVehicleQuery { Id = vehicleId };

        _mockRepository.Setup(r => r.GetByIdAsync(vehicleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicle);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(vehicleId);
        result.LicensePlate.Should().Be("ABC-1234");
    }

    [Fact]
    public async Task Handle_NonExistentVehicle_ShouldReturnNull()
    {
        var vehicleId = Guid.NewGuid();
        var query = new GetVehicleQuery { Id = vehicleId };

        _mockRepository.Setup(r => r.GetByIdAsync(vehicleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Vehicle?)null);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeNull();
    }
}
