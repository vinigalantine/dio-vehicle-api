using DioVehicleApi.Application.Features.Auth.Commands.Login;
using FluentAssertions;

namespace DioVehicleApi.Application.UnitTests.Contracts.Auth;

public class LoginResponseTests
{
    [Fact]
    public void LoginResponse_WithValidProperties_ShouldSetCorrectly()
    {
        var expiresAt = DateTime.UtcNow.AddHours(24);
        var response = new LoginResponse
        {
            Token = "test-token-12345",
            ExpiresAt = expiresAt
        };

        response.Token.Should().Be("test-token-12345");
        response.ExpiresAt.Should().Be(expiresAt);
    }
}
