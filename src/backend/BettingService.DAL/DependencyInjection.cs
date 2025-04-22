using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.Models.Settings;
using BettingService.DAL.Repositories;
using BettingService.DAL.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BettingService.DAL;

public static class DependencyInjection
{
    public static IServiceCollection AddDataAccessLayer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetSection("DatabaseSettings")["ConnectionString"];
        
        services.AddDbContext<RepositoryContext>(options => 
            options.UseNpgsql(connectionString));
        
        services.AddScoped<IBetRepository, BetRepository>();
        return services;
    }
}