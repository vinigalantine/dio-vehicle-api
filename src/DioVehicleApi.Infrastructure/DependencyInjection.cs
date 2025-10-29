using DioVehicleApi.Domain.Interfaces.Repositories;
using DioVehicleApi.Domain.Services;
using DioVehicleApi.Infrastructure.Data;
using DioVehicleApi.Infrastructure.Data.Repositories;
using DioVehicleApi.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DioVehicleApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<IBrandRepository, BrandRepository>();
        services.AddScoped<IModelRepository, ModelRepository>();
        services.AddScoped<IVehicleRepository, VehicleRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // Services
        services.AddSingleton<IPasswordHasher, Md5PasswordHasher>();

        // MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(
            typeof(DioVehicleApi.Application.Features.Auth.Commands.Login.LoginCommand).Assembly));

        return services;
    }
}
