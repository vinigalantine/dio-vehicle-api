using DioVehicleApi.Application.Features.Models.Queries.GetAllModels;
using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Interfaces.Repositories;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace DioVehicleApi.Application.UnitTests.Features.Models.Queries.GetAllModels;

public class GetAllModelsQueryHandlerTests
{
    private readonly Mock<IModelRepository> _mockRepository;
    private readonly GetAllModelsQueryHandler _handler;

    public GetAllModelsQueryHandlerTests()
    {
        _mockRepository = new Mock<IModelRepository>();
        _handler = new GetAllModelsQueryHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ShouldReturnPaginatedResult()
    {
        var models = new List<Model>
        {
            new Model { Id = Guid.NewGuid(), Name = "Corolla", BrandId = Guid.NewGuid() },
            new Model { Id = Guid.NewGuid(), Name = "Civic", BrandId = Guid.NewGuid() }
        };
        var query = new GetAllModelsQuery { PageNumber = 1, PageSize = 10 };

        _mockRepository.Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<Model, bool>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(models);
        _mockRepository.Setup(r => r.CountAsync(
                It.IsAny<Expression<Func<Model, bool>>>(),
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
        var query = new GetAllModelsQuery { PageNumber = 1, PageSize = 10 };

        _mockRepository.Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<Model, bool>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Model>());
        _mockRepository.Setup(r => r.CountAsync(
                It.IsAny<Expression<Func<Model, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
}
