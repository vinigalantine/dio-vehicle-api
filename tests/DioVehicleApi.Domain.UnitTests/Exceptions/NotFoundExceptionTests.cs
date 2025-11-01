using DioVehicleApi.Domain.Exceptions;
using FluentAssertions;

namespace DioVehicleApi.Domain.UnitTests.Exceptions;

public class NotFoundExceptionTests
{
    [Fact]
    public void Constructor_WithMessage_ShouldCreateException()
    {
        var message = "Brand with ID 123 was not found";

        var exception = new NotFoundException(message);

        exception.Should().NotBeNull();
        exception.Message.Should().Be(message);
        exception.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_ShouldCreateException()
    {
        var message = "Vehicle not found";
        var innerException = new InvalidOperationException("Database error");

        var exception = new NotFoundException(message, innerException);

        exception.Should().NotBeNull();
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
    }

    [Theory]
    [InlineData("Brand not found")]
    [InlineData("Model with ID abc123 does not exist")]
    [InlineData("Vehicle with license plate XYZ-1234 not found")]
    public void Constructor_WithDifferentMessages_ShouldStoreCorrectly(string message)
    {
        var exception = new NotFoundException(message);

        exception.Message.Should().Be(message);
    }

    [Fact]
    public void Exception_ShouldBeCatchableAsBaseException()
    {
        var message = "Entity not found";

        Action act = () => throw new NotFoundException(message);

        act.Should().Throw<Exception>()
            .WithMessage(message);
    }

    [Fact]
    public void Exception_ShouldBeCatchableAsNotFoundException()
    {
        var message = "Brand not found";

        Action act = () => throw new NotFoundException(message);

        act.Should().Throw<NotFoundException>()
            .WithMessage(message);
    }

    [Fact]
    public void Exception_InnerException_ShouldBeAccessible()
    {
        var innerMessage = "Database connection failed";
        var innerException = new Exception(innerMessage);
        var exception = new NotFoundException("Entity not found", innerException);

        exception.InnerException.Should().NotBeNull();
        exception.InnerException.Should().BeOfType<Exception>();
        exception.InnerException!.Message.Should().Be(innerMessage);
    }
}
