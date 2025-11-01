using DioVehicleApi.Application.Features.Vehicles.Queries.GetAllVehicles;
using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Interfaces.Repositories;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace DioVehicleApi.Application.UnitTests.Features.Vehicles.Queries.GetAllVehicles;

public class GetAllVehiclesQueryHandlerTests
{
    private readonly Mock<IVehicleRepository> _mockRepository;
    private readonly GetAllVehiclesQueryHandler _handler;

    public GetAllVehiclesQueryHandlerTests()
    {
        _mockRepository = new Mock<IVehicleRepository>();
        _handler = new GetAllVehiclesQueryHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ShouldReturnPaginatedResult()
    {
        var vehicles = new List<Vehicle>
        {
            new Vehicle { Id = Guid.NewGuid(), ModelId = Guid.NewGuid(), Year = 2023, Color = "White", LicensePlate = "ABC-1234" },
            new Vehicle { Id = Guid.NewGuid(), ModelId = Guid.NewGuid(), Year = 2022, Color = "Black", LicensePlate = "XYZ-5678" }
        };
        var query = new GetAllVehiclesQuery { PageNumber = 1, PageSize = 10 };

        _mockRepository.Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<Vehicle, bool>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicles);
        _mockRepository.Setup(r => r.CountAsync(
                It.IsAny<Expression<Func<Vehicle, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_EmptyResult_ShouldReturnEmptyList()
    {
        var query = new GetAllVehiclesQuery { PageNumber = 1, PageSize = 10 };

        _mockRepository.Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<Vehicle, bool>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Vehicle>());
        _mockRepository.Setup(r => r.CountAsync(
                It.IsAny<Expression<Func<Vehicle, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
}
