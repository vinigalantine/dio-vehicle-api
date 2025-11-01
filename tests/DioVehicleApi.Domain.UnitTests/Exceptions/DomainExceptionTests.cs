using DioVehicleApi.Domain.Exceptions;
using FluentAssertions;

namespace DioVehicleApi.Domain.UnitTests.Exceptions;

public class DomainExceptionTests
{
    [Fact]
    public void DomainException_WithMessage_ShouldStoreMessage()
    {
        var message = "Domain validation failed";

        var exception = new DomainException(message);

        exception.Should().NotBeNull();
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void DomainException_WithMessageAndInnerException_ShouldStoreBoth()
    {
        var message = "Domain operation failed";
        var innerException = new InvalidOperationException("Inner error");

        var exception = new DomainException(message, innerException);

        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
        exception.InnerException.Should().BeOfType<InvalidOperationException>();
    }

    [Fact]
    public void DomainException_ShouldBeThrowable()
    {
        var message = "Test domain exception";

        Action act = () => throw new DomainException(message);

        act.Should().Throw<DomainException>()
            .WithMessage(message);
    }
}
