using DioVehicleApi.Application.Features.Brands.Commands.CreateBrand;
using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Interfaces.Repositories;
using FluentAssertions;
using Moq;

namespace DioVehicleApi.Application.UnitTests.Features.Brands.Commands.CreateBrand;

public class CreateBrandCommandHandlerTests
{
    private readonly Mock<IBrandRepository> _mockRepository;
    private readonly CreateBrandCommandHandler _handler;

    public CreateBrandCommandHandlerTests()
    {
        _mockRepository = new Mock<IBrandRepository>();
        _handler = new CreateBrandCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateBrandSuccessfully()
    {
        var command = new CreateBrandCommand { Name = "Toyota" };
        var brandId = Guid.NewGuid();
        var expectedBrand = new Brand
        {
            Id = brandId,
            Name = "Toyota"
        };

        _mockRepository
            .Setup(x => x.CreateAsync(It.IsAny<Brand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBrand);

        _mockRepository
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be("Toyota");
        result.Id.Should().NotBeEmpty();

        _mockRepository.Verify(
            x => x.CreateAsync(It.Is<Brand>(b => b.Name == "Toyota"), It.IsAny<CancellationToken>()),
            Times.Once);

        _mockRepository.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryCreatesCalled_ShouldPassCorrectEntity()
    {
        var command = new CreateBrandCommand { Name = "Mercedes" };
        Brand? capturedBrand = null;

        _mockRepository
            .Setup(x => x.CreateAsync(It.IsAny<Brand>(), It.IsAny<CancellationToken>()))
            .Callback<Brand, CancellationToken>((brand, ct) => capturedBrand = brand)
            .ReturnsAsync((Brand b, CancellationToken ct) => b);

        _mockRepository
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _handler.Handle(command, CancellationToken.None);

        capturedBrand.Should().NotBeNull();
        capturedBrand!.Name.Should().Be("Mercedes");
        capturedBrand.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_SaveChangesFails_ShouldThrowException()
    {
        var command = new CreateBrandCommand { Name = "Audi" };
        var brand = new Brand { Id = Guid.NewGuid(), Name = "Audi" };

        _mockRepository
            .Setup(x => x.CreateAsync(It.IsAny<Brand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(brand);

        _mockRepository
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Database error");
    }

    [Fact]
    public async Task Handle_CancellationRequested_ShouldRespectCancellationToken()
    {
        var command = new CreateBrandCommand { Name = "Volvo" };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        _mockRepository
            .Setup(x => x.CreateAsync(It.IsAny<Brand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        Func<Task> act = async () => await _handler.Handle(command, cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
