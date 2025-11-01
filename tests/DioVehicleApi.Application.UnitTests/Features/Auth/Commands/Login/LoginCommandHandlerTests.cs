using DioVehicleApi.Application.Configuration;
using DioVehicleApi.Application.Features.Auth.Commands.Login;
using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Interfaces.Repositories;
using DioVehicleApi.Domain.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace DioVehicleApi.Application.UnitTests.Features.Auth.Commands.Login;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;
    private readonly Mock<IOptions<JwtSettings>> _mockJwtSettings;
    private readonly Mock<ILogger<LoginCommandHandler>> _mockLogger;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _mockJwtSettings = new Mock<IOptions<JwtSettings>>();
        _mockLogger = new Mock<ILogger<LoginCommandHandler>>();
        
        var jwtSettings = new JwtSettings
        {
            SecretKey = "ThisIsAVerySecretKeyForTestingPurposesOnly123456789",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpirationHours = 24
        };
        _mockJwtSettings.Setup(x => x.Value).Returns(jwtSettings);
        
        _handler = new LoginCommandHandler(_mockUserRepository.Object, _mockPasswordHasher.Object, _mockJwtSettings.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ShouldReturnLoginResponse()
    {
        var user = new User { Id = Guid.NewGuid(), Username = "admin", PasswordHash = "hashedpassword", IsAdmin = true };
        var command = new LoginCommand { Username = "admin", Password = "password" };

        _mockUserRepository.Setup(r => r.GetByUsernameAsync("admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mockPasswordHasher.Setup(p => p.Verify("password", "hashedpassword"))
            .Returns(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrEmpty();
        result.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task Handle_UserNotFound_ShouldReturnNull()
    {
        var command = new LoginCommand { Username = "nonexistent", Password = "password" };

        _mockUserRepository.Setup(r => r.GetByUsernameAsync("nonexistent", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_InvalidPassword_ShouldReturnNull()
    {
        var user = new User { Id = Guid.NewGuid(), Username = "admin", PasswordHash = "hashedpassword", IsAdmin = false };
        var command = new LoginCommand { Username = "admin", Password = "wrongpassword" };

        _mockUserRepository.Setup(r => r.GetByUsernameAsync("admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mockPasswordHasher.Setup(p => p.Verify("wrongpassword", "hashedpassword"))
            .Returns(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
    }
}
