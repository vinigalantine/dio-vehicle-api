using FluentAssertions;
using DioVehicleApi.Infrastructure.Services;

namespace DioVehicleApi.Infrastructure.UnitTests.Services;

public class Md5PasswordHasherTests
{
    private readonly Md5PasswordHasher _hasher;

    public Md5PasswordHasherTests()
    {
        _hasher = new Md5PasswordHasher();
    }

    [Fact]
    public void HashPassword_ValidPassword_ShouldReturnHash()
    {
        var password = "MySecurePassword123";

        var hash = _hasher.Hash(password);

        hash.Should().NotBeNullOrEmpty();
        hash.Length.Should().Be(32);
        hash.Should().MatchRegex("^[a-f0-9]{32}$");
    }

    [Fact]
    public void HashPassword_SamePassword_ShouldAlwaysReturnSameHash()
    {
        var password = "TestPassword";

        var hash1 = _hasher.Hash(password);
        var hash2 = _hasher.Hash(password);

        hash1.Should().Be(hash2);
    }

    [Fact]
    public void HashPassword_DifferentPasswords_ShouldReturnDifferentHashes()
    {
        var password1 = "password123";
        var password2 = "DifferentPassword";

        var hash1 = _hasher.Hash(password1);
        var hash2 = _hasher.Hash(password2);

        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void HashPassword_EmptyString_ShouldReturnHash()
    {
        var password = "";

        var hash = _hasher.Hash(password);

        hash.Should().NotBeNullOrEmpty();
        hash.Length.Should().Be(32);
    }

    [Fact]
    public void HashPassword_SpecialCharacters_ShouldHandleCorrectly()
    {
        var password = "P@$$w0rd!#%&*()パスワード123" + new string('a', 100);

        var hash = _hasher.Hash(password);

        hash.Should().NotBeNullOrEmpty();
        hash.Should().MatchRegex("^[a-f0-9]{32}$");
    }

    [Theory]
    [InlineData("admin", "21232f297a57a5a743894a0e4a801fc3")]
    [InlineData("password", "5f4dcc3b5aa765d61d8327deb882cf99")]
    public void HashPassword_KnownValues_ShouldMatchExpectedHash(string password, string expectedHash)
    {
        var hash = _hasher.Hash(password);

        hash.Should().Be(expectedHash);
    }

    [Fact]
    public void VerifyPassword_CorrectPassword_ShouldReturnTrue()
    {
        var password = "MyPassword123";
        var hash = _hasher.Hash(password);

        var isValid = _hasher.Verify(password, hash);

        isValid.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_IncorrectPassword_ShouldReturnFalse()
    {
        var correctPassword = "CorrectPassword";
        var wrongPassword = "WrongPassword";
        var hash = _hasher.Hash(correctPassword);

        var isValid = _hasher.Verify(wrongPassword, hash);

        isValid.Should().BeFalse();
    }
}
