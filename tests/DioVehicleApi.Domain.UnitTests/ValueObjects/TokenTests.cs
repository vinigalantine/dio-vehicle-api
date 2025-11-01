using DioVehicleApi.Domain.ValueObjects;
using FluentAssertions;

namespace DioVehicleApi.Domain.UnitTests.ValueObjects;

public class TokenTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateToken()
    {
        var tokenValue = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.test";
        var expiresAt = DateTime.UtcNow.AddHours(1);

        var token = new Token(tokenValue, expiresAt);

        token.Should().NotBeNull();
        token.Value.Should().Be(tokenValue);
        token.ExpiresAt.Should().Be(expiresAt);
    }

    [Fact]
    public void Token_ShouldBeImmutable()
    {
        var tokenValue = "test-token";
        var expiresAt = DateTime.UtcNow.AddHours(1);

        var token = new Token(tokenValue, expiresAt);

        token.Value.Should().Be(tokenValue);
        token.ExpiresAt.Should().Be(expiresAt);
    }

    [Fact]
    public void Token_WithExpirationInPast_ShouldStillCreate()
    {
        var tokenValue = "expired-token";
        var expiresAt = DateTime.UtcNow.AddHours(-1);

        var token = new Token(tokenValue, expiresAt);

        token.Should().NotBeNull();
        token.ExpiresAt.Should().BeBefore(DateTime.UtcNow);
    }

    [Theory]
    [InlineData("")]
    [InlineData("short")]
    [InlineData("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c")]
    public void Token_WithDifferentValues_ShouldCreate(string tokenValue)
    {
        var expiresAt = DateTime.UtcNow.AddHours(1);

        var token = new Token(tokenValue, expiresAt);

        token.Value.Should().Be(tokenValue);
    }

    [Fact]
    public void Token_Equality_SameValues_ShouldBeEqual()
    {
        var tokenValue = "test-token";
        var expiresAt = DateTime.UtcNow.AddHours(1);

        var token1 = new Token(tokenValue, expiresAt);
        var token2 = new Token(tokenValue, expiresAt);

        token1.Should().Be(token2);
        (token1 == token2).Should().BeTrue();
    }

    [Fact]
    public void Token_Equality_DifferentValues_ShouldNotBeEqual()
    {
        var expiresAt = DateTime.UtcNow.AddHours(1);

        var token1 = new Token("token1", expiresAt);
        var token2 = new Token("token2", expiresAt);

        token1.Should().NotBe(token2);
        (token1 == token2).Should().BeFalse();
    }

    [Fact]
    public void Token_Deconstruction_ShouldWorkCorrectly()
    {
        var tokenValue = "test-token";
        var expiresAt = DateTime.UtcNow.AddHours(1);
        var token = new Token(tokenValue, expiresAt);

        var (value, expires) = token;

        value.Should().Be(tokenValue);
        expires.Should().Be(expiresAt);
    }
}
