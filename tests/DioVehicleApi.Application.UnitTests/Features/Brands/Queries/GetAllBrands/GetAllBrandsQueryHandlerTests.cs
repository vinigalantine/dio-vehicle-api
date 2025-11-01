using DioVehicleApi.Application.Contracts.Base;
using DioVehicleApi.Application.Features.Brands.Queries.GetAllBrands;
using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Interfaces.Repositories;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace DioVehicleApi.Application.UnitTests.Features.Brands.Queries.GetAllBrands;

public class GetAllBrandsQueryHandlerTests
{
    private readonly Mock<IBrandRepository> _mockRepository;
    private readonly GetAllBrandsQueryHandler _handler;

    public GetAllBrandsQueryHandlerTests()
    {
        _mockRepository = new Mock<IBrandRepository>();
        _handler = new GetAllBrandsQueryHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ShouldReturnPaginatedResult()
    {
        var brands = new List<Brand>
        {
            new Brand { Id = Guid.NewGuid(), Name = "Toyota" },
            new Brand { Id = Guid.NewGuid(), Name = "Honda" }
        };
        var query = new GetAllBrandsQuery { PageNumber = 1, PageSize = 10 };

        _mockRepository.Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<Brand, bool>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(brands);
        _mockRepository.Setup(r => r.CountAsync(
                It.IsAny<Expression<Func<Brand, bool>>>(),
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
        var query = new GetAllBrandsQuery { PageNumber = 1, PageSize = 10 };

        _mockRepository.Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<Brand, bool>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Brand>());
        _mockRepository.Setup(r => r.CountAsync(
                It.IsAny<Expression<Func<Brand, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WithNameFilter_ShouldPassFilterToRepository()
    {
        var brands = new List<Brand>
        {
            new Brand { Id = Guid.NewGuid(), Name = "Toyota" }
        };
        var query = new GetAllBrandsQuery { Name = "Toyota", PageNumber = 1, PageSize = 10 };
        Expression<Func<Brand, bool>>? capturedFilter = null;

        _mockRepository.Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<Brand, bool>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .Callback<Expression<Func<Brand, bool>>?, int?, int?, CancellationToken>((f, skip, take, ct) => capturedFilter = f)
            .ReturnsAsync(brands);
        _mockRepository.Setup(r => r.CountAsync(
                It.IsAny<Expression<Func<Brand, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _handler.Handle(query, CancellationToken.None);

        capturedFilter.Should().NotBeNull();
        var testBrand = new Brand { Id = Guid.NewGuid(), Name = "Toyota" };
        capturedFilter!.Compile()(testBrand).Should().BeTrue();
    }

    [Theory]
    [InlineData(1, 10, 0, 10)]
    [InlineData(2, 10, 10, 10)]
    [InlineData(3, 20, 40, 20)]
    [InlineData(1, 5, 0, 5)]
    public async Task Handle_WithPagination_ShouldCalculateSkipAndTakeCorrectly(
        int pageNumber, int pageSize, int expectedSkip, int expectedTake)
    {
        var query = new GetAllBrandsQuery { PageNumber = pageNumber, PageSize = pageSize };
        int? capturedSkip = null;
        int? capturedTake = null;

        _mockRepository.Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<Brand, bool>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .Callback<Expression<Func<Brand, bool>>?, int?, int?, CancellationToken>((f, skip, take, ct) =>
            {
                capturedSkip = skip;
                capturedTake = take;
            })
            .ReturnsAsync(new List<Brand>());
        _mockRepository.Setup(r => r.CountAsync(
                It.IsAny<Expression<Func<Brand, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        await _handler.Handle(query, CancellationToken.None);

        capturedSkip.Should().Be(expectedSkip);
        capturedTake.Should().Be(expectedTake);
    }

    [Fact]
    public async Task Handle_WithCancellation_ShouldPassCancellationToken()
    {
        var query = new GetAllBrandsQuery { PageNumber = 1, PageSize = 10 };
        var cancellationToken = new CancellationToken();

        _mockRepository.Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<Brand, bool>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                cancellationToken))
            .ReturnsAsync(new List<Brand>());
        _mockRepository.Setup(r => r.CountAsync(
                It.IsAny<Expression<Func<Brand, bool>>>(),
                cancellationToken))
            .ReturnsAsync(0);

        await _handler.Handle(query, cancellationToken);

        _mockRepository.Verify(r => r.GetAllAsync(
            It.IsAny<Expression<Func<Brand, bool>>>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            cancellationToken), Times.Once);
        _mockRepository.Verify(r => r.CountAsync(
            It.IsAny<Expression<Func<Brand, bool>>>(),
            cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_EmptyNameFilter_ShouldReturnAllBrands()
    {
        var brands = new List<Brand>
        {
            new Brand { Id = Guid.NewGuid(), Name = "Toyota" },
            new Brand { Id = Guid.NewGuid(), Name = "Honda" },
            new Brand { Id = Guid.NewGuid(), Name = "Ford" }
        };
        var query = new GetAllBrandsQuery { Name = string.Empty, PageNumber = 1, PageSize = 10 };

        _mockRepository.Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<Brand, bool>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(brands);
        _mockRepository.Setup(r => r.CountAsync(
                It.IsAny<Expression<Func<Brand, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(3);
        result.TotalCount.Should().Be(3);
    }

    [Fact]
    public async Task Handle_ShouldReturnPaginatedResultType()
    {
        var query = new GetAllBrandsQuery { PageNumber = 1, PageSize = 10 };

        _mockRepository.Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<Brand, bool>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Brand>());
        _mockRepository.Setup(r => r.CountAsync(
                It.IsAny<Expression<Func<Brand, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeOfType<PaginatedResult<Brand>>();
        result.Should().NotBeNull();
    }
}
