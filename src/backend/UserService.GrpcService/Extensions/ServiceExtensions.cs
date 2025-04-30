using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.Yaml;
using UserService.Domain.Models;
using UserService.Domain.RepositoryContracts;
using UserService.GrpcService.Exceptions;
using UserService.Infrastructure.Repository;
using UserService.Infrastructure.Repository.Repositories;

namespace UserService.GrpcService.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["DatabaseSettings:ConnectionString"]
                               ?? throw new InvalidOperationException("Connection string is missing");

        services.AddDbContext<RepositoryContext>(opts =>
            opts.UseNpgsql(connectionString));
    }

    public static void ConfigureGrpc(this IServiceCollection services)
    {
        services.AddGrpc(options =>
        {
            options.EnableDetailedErrors = true;
            options.Interceptors.Add<ExceptionInterceptor>();
        });
    }
    
    public static void AddRepository(this IServiceCollection services)
    {
        services.AddScoped<IUsersRepository, UserRepository>();
    }

    public static void ConfigureIdentity(this IServiceCollection services)
    {
        services.AddIdentityCore<User>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<RepositoryContext>();
    }
}