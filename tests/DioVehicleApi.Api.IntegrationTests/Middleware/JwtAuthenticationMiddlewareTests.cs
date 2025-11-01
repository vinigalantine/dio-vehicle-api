using DioVehicleApi.Application.Configuration;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace DioVehicleApi.Api.IntegrationTests.Middleware;

public class JwtAuthenticationMiddlewareTests
{
    private readonly Mock<ILogger<JwtAuthenticationMiddleware>> _mockLogger;
    private readonly JwtSettings _jwtSettings;
    private readonly Mock<RequestDelegate> _mockNext;

    public JwtAuthenticationMiddlewareTests()
    {
        _mockLogger = new Mock<ILogger<JwtAuthenticationMiddleware>>();
        _jwtSettings = new JwtSettings
        {
            SecretKey = "ThisIsAVerySecretKeyForTestingPurposesOnly12345",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpirationHours = 1
        };
        _mockNext = new Mock<RequestDelegate>();
    }

    [Fact]
    public async Task InvokeAsync_NoAuthorizationHeader_ShouldCallNextMiddleware()
    {
        var context = CreateHttpContext();
        var middleware = CreateMiddleware();

        await middleware.InvokeAsync(context);

        _mockNext.Verify(x => x(context), Times.Once);
        context.User.Identity?.IsAuthenticated.Should().BeFalse();
    }

    [Fact]
    public async Task InvokeAsync_ValidToken_ShouldAuthenticateUser()
    {
        var token = GenerateValidToken("testuser");
        var context = CreateHttpContext();
        context.Request.Headers["Authorization"] = $"Bearer {token}";
        var middleware = CreateMiddleware();

        await middleware.InvokeAsync(context);

        _mockNext.Verify(x => x(context), Times.Once);
        context.User.Identity?.IsAuthenticated.Should().BeTrue();
        context.User.FindFirst("username")?.Value.Should().Be("testuser");
    }

    [Fact]
    public async Task InvokeAsync_ExpiredToken_ShouldReturn401()
    {
        var token = GenerateExpiredToken("testuser");
        var context = CreateHttpContext();
        context.Request.Headers["Authorization"] = $"Bearer {token}";
        var middleware = CreateMiddleware();

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task InvokeAsync_InvalidToken_ShouldNotAuthenticate()
    {
        var context = CreateHttpContext();
        context.Request.Headers["Authorization"] = "Bearer invalid.token.here";
        var middleware = CreateMiddleware();

        await middleware.InvokeAsync(context);

        _mockNext.Verify(x => x(context), Times.Once);
        context.User.Identity?.IsAuthenticated.Should().BeFalse();
    }

    [Fact]
    public async Task InvokeAsync_MalformedAuthorizationHeader_ShouldNotAuthenticate()
    {
        var context = CreateHttpContext();
        context.Request.Headers["Authorization"] = "InvalidFormat token123";
        var middleware = CreateMiddleware();

        await middleware.InvokeAsync(context);

        _mockNext.Verify(x => x(context), Times.Once);
        context.User.Identity?.IsAuthenticated.Should().BeFalse();
    }

    [Fact]
    public async Task InvokeAsync_TokenWithWrongIssuer_ShouldNotAuthenticate()
    {
        var token = GenerateTokenWithWrongIssuer("testuser");
        var context = CreateHttpContext();
        context.Request.Headers["Authorization"] = $"Bearer {token}";
        var middleware = CreateMiddleware();

        await middleware.InvokeAsync(context);

        _mockNext.Verify(x => x(context), Times.Once);
        context.User.Identity?.IsAuthenticated.Should().BeFalse();
    }

    [Fact]
    public async Task InvokeAsync_EmptyBearerToken_ShouldNotAuthenticate()
    {
        var context = CreateHttpContext();
        context.Request.Headers["Authorization"] = "Bearer ";
        var middleware = CreateMiddleware();

        await middleware.InvokeAsync(context);

        _mockNext.Verify(x => x(context), Times.Once);
        context.User.Identity?.IsAuthenticated.Should().BeFalse();
    }

    [Theory]
    [InlineData("user1")]
    [InlineData("admin")]
    [InlineData("testuser@example.com")]
    public async Task InvokeAsync_ValidTokenForDifferentUsers_ShouldAuthenticate(string username)
    {
        var token = GenerateValidToken(username);
        var context = CreateHttpContext();
        context.Request.Headers["Authorization"] = $"Bearer {token}";
        var middleware = CreateMiddleware();

        await middleware.InvokeAsync(context);

        context.User.Identity?.IsAuthenticated.Should().BeTrue();
        context.User.FindFirst("username")?.Value.Should().Be(username);
    }

    private JwtAuthenticationMiddleware CreateMiddleware()
    {
        var options = Options.Create(_jwtSettings);
        return new JwtAuthenticationMiddleware(_mockNext.Object, options, _mockLogger.Object);
    }

    private HttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }

    private string GenerateValidToken(string username)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("username", username) }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateExpiredToken(string username)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("username", username) }),
            NotBefore = DateTime.UtcNow.AddHours(-2), // Issued 2 hours ago
            Expires = DateTime.UtcNow.AddHours(-1), // Expired 1 hour ago
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateTokenWithWrongIssuer(string username)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("username", username) }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = "WrongIssuer",
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
