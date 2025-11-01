using DioVehicleApi.Domain.Entities;
using DioVehicleApi.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DioVehicleApi.Api.IntegrationTests.Infrastructure;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("dev");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            // Add test configuration for JWT settings
            var testConfig = new Dictionary<string, string?>
            {
                ["JwtSettings:SecretKey"] = "ThisIsAVerySecretKeyForTestingPurposesOnly12345",
                ["JwtSettings:Issuer"] = "TestIssuer",
                ["JwtSettings:Audience"] = "TestAudience",
                ["JwtSettings:ExpirationHours"] = "1",
                ["IsTest"] = "true"
            };
            config.AddInMemoryCollection(testConfig);
        });

        builder.ConfigureTestServices(services =>
        {
            services.AddHttpContextAccessor();

            // Remove all database related services to avoid provider conflicts
            services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));
            services.RemoveAll(typeof(ApplicationDbContext));
            
            // Remove any existing database provider services and related EF Core services
            var descriptorsToRemove = services.Where(d => 
                d.ServiceType.Namespace?.Contains("EntityFrameworkCore") == true ||
                d.ServiceType.FullName?.Contains("Microsoft.EntityFrameworkCore") == true ||
                d.ServiceType.Name.Contains("SqlServer") ||
                d.ServiceType.Name.Contains("InMemory") ||
                d.ServiceType.Name.Contains("Database") ||
                d.ServiceType == typeof(Domain.Interfaces.Repositories.IBrandRepository) ||
                d.ServiceType == typeof(Domain.Interfaces.Repositories.IModelRepository) ||
                d.ServiceType == typeof(Domain.Interfaces.Repositories.IVehicleRepository) ||
                d.ServiceType == typeof(Domain.Interfaces.Repositories.IUserRepository) ||
                d.ServiceType == typeof(Domain.Services.IPasswordHasher)
            ).ToList();
            
            foreach (var descriptor in descriptorsToRemove)
            {
                services.Remove(descriptor);
            }

            // Add in-memory database for testing
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("IntegrationTestDb");
                options.EnableSensitiveDataLogging();
            });

            services.AddScoped<Domain.Interfaces.Repositories.IBrandRepository, DioVehicleApi.Infrastructure.Data.Repositories.BrandRepository>();
            services.AddScoped<Domain.Interfaces.Repositories.IModelRepository, DioVehicleApi.Infrastructure.Data.Repositories.ModelRepository>();
            services.AddScoped<Domain.Interfaces.Repositories.IVehicleRepository, DioVehicleApi.Infrastructure.Data.Repositories.VehicleRepository>();
            services.AddScoped<Domain.Interfaces.Repositories.IUserRepository, DioVehicleApi.Infrastructure.Data.Repositories.UserRepository>();

            services.AddSingleton<Domain.Services.IPasswordHasher, DioVehicleApi.Infrastructure.Services.Md5PasswordHasher>();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(
                typeof(Application.Features.Auth.Commands.Login.LoginCommand).Assembly));

            var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<ApplicationDbContext>();

            // Ensure database is created
            db.Database.EnsureCreated();

            // Seed test data
            SeedTestData(db);
        });
    }

    public static void SeedTestData(ApplicationDbContext context)
    {
        // Clear existing data
        context.Brands.RemoveRange(context.Brands);
        context.Models.RemoveRange(context.Models);
        context.Vehicles.RemoveRange(context.Vehicles);
        context.Users.RemoveRange(context.Users);
        context.SaveChanges();

        var adminUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "admin",
            PasswordHash = HashPassword("admin123"),
            IsAdmin = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        var normalUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "user",
            PasswordHash = HashPassword("user123"),
            IsAdmin = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        context.Users.AddRange(adminUser, normalUser);

        var toyotaBrand = new Brand
        {
            Id = Guid.NewGuid(),
            Name = "Toyota",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        var hondaBrand = new Brand
        {
            Id = Guid.NewGuid(),
            Name = "Honda",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        context.Brands.AddRange(toyotaBrand, hondaBrand);

        var corollaModel = new Model
        {
            Id = Guid.NewGuid(),
            Name = "Corolla",
            BrandId = toyotaBrand.Id,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        var civicModel = new Model
        {
            Id = Guid.NewGuid(),
            Name = "Civic",
            BrandId = hondaBrand.Id,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        context.Models.AddRange(corollaModel, civicModel);

        var vehicle1 = new Vehicle
        {
            Id = Guid.NewGuid(),
            ModelId = corollaModel.Id,
            Year = 2023,
            Color = "White",
            LicensePlate = "ABC-1234",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        var vehicle2 = new Vehicle
        {
            Id = Guid.NewGuid(),
            ModelId = civicModel.Id,
            Year = 2022,
            Color = "Black",
            LicensePlate = "XYZ-5678",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        context.Vehicles.AddRange(vehicle1, vehicle2);

        context.SaveChanges();
    }

    private static string HashPassword(string password)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();
        var inputBytes = System.Text.Encoding.UTF8.GetBytes(password);
        var hashBytes = md5.ComputeHash(inputBytes);
        return Convert.ToHexString(hashBytes).ToLower();
    }
}
