using DioVehicleApi.Application.Features.Models.Queries.GetModel;
using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Interfaces.Repositories;
using FluentAssertions;
using Moq;

namespace DioVehicleApi.Application.UnitTests.Features.Models.Queries.GetModel;

public class GetModelQueryHandlerTests
{
    private readonly Mock<IModelRepository> _mockRepository;
    private readonly GetModelQueryHandler _handler;

    public GetModelQueryHandlerTests()
    {
        _mockRepository = new Mock<IModelRepository>();
        _handler = new GetModelQueryHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ExistingModel_ShouldReturnModel()
    {
        var modelId = Guid.NewGuid();
        var model = new Model { Id = modelId, Name = "Corolla", BrandId = Guid.NewGuid() };
        var query = new GetModelQuery { Id = modelId };

        _mockRepository.Setup(r => r.GetByIdAsync(modelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(model);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(modelId);
        result.Name.Should().Be("Corolla");
    }

    [Fact]
    public async Task Handle_NonExistentModel_ShouldReturnNull()
    {
        var modelId = Guid.NewGuid();
        var query = new GetModelQuery { Id = modelId };

        _mockRepository.Setup(r => r.GetByIdAsync(modelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Model?)null);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeNull();
    }
}
