using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace DioVehicleApi.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        // Check if users already exist
        var usersExist = await context.Users.AnyAsync();
        
        if (!usersExist)
        {
            var users = new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "admin",
                    PasswordHash = passwordHasher.Hash("Admin@123"),
                    IsAdmin = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "user1",
                    PasswordHash = passwordHasher.Hash("User1@123"),
                    IsAdmin = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }
    }
}
