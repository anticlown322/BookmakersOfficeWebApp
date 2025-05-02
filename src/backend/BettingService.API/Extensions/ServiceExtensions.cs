using System.Reflection;
using System.Text;
using BettingService.API.Utility;
using BettingService.BLL.Contracts.Services;
using BettingService.BLL.Services;
using BettingService.BLL.Utility;
using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.Models.Settings;
using BettingService.DAL.Repositories;
using BettingService.DAL.Repositories.Implementations;
using BettingService.Protos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NLog;

namespace BettingService.API.Extensions;

public static class ServiceExtensions
{
    public static void AddDockerSecrets(this IConfigurationBuilder config)
    {
        const string secretsPath = "/run/secrets/";
        if (Directory.Exists(secretsPath))
        {
            foreach (var file in Directory.GetFiles(secretsPath))
            {
                config.AddKeyPerFile(file, optional: true);
            }
        }
    }

    public static void AddAppSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseSettings>(configuration.GetSection("DatabaseSettings"));
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.Configure<HangfireSettings>(configuration.GetSection("HangfireSettings"));
        services.Configure<GrpcSettings>(configuration.GetSection("GrpcSettings"));
    }

    public static void ConfigureNLog(this IServiceCollection services)
    {
        const string configPath = "/app/Properties/nlog.config";
        if (File.Exists(configPath))
        {
            LogManager.Setup().LoadConfigurationFromFile(configPath);
            return;
        }

        throw new FileNotFoundException($"NLog config not found at: {configPath}");
    }

    public static void ConfigureAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer();

        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
        services.PostConfigure<JwtBearerOptions>(
            JwtBearerDefaults.AuthenticationScheme,
            options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey!)),
                    NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
                    RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                };
            });
    }

    public static void AddAuthorizationPolicies(this IServiceCollection services) =>
        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                AuthorizationPolicies.AdministratorOnly,
                policy =>
                    policy.RequireRole(UserRoles.Administrator));
            options.AddPolicy(
                AuthorizationPolicies.GamblerOnly,
                policy =>
                    policy.RequireRole(UserRoles.Gambler));
        });

    public static void AddGrpcClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpcClient<UserGrpcService.UserGrpcServiceClient>(o =>
        {
            o.Address = new Uri("http://localhost:50023");
        });

        services.AddGrpcClient<SportDataService.SportDataServiceClient>(o =>
        {
            o.Address = new Uri("http://localhost:50022");
        });
    }

    public static void ConfigureApiBehaviorOptions(this IServiceCollection services) =>
        services.Configure<ApiBehaviorOptions>(opt => { opt.SuppressModelStateInvalidFilter = true; });
}