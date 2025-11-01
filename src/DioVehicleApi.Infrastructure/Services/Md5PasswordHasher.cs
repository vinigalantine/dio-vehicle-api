using System.Security.Cryptography;
using System.Text;
using DioVehicleApi.Domain.Services;

namespace DioVehicleApi.Infrastructure.Services;

/// <summary>
/// Using a simple MD5 just because it's a simple "portfolio app"
/// For production I would BCrypt, Argon2, or PBKDF2.
/// </summary>
public class Md5PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        using (var md5 = MD5.Create())
        {
            var inputBytes = Encoding.UTF8.GetBytes(password);
            var hashBytes = md5.ComputeHash(inputBytes);
            
            return Convert.ToHexString(hashBytes).ToLower();
        }
    }

    public bool Verify(string password, string hash)
    {
        var hashedPassword = Hash(password);
        return hashedPassword == hash;
    }
}
