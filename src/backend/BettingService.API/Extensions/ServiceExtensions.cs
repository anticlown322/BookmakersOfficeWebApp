using System.Security.Cryptography.X509Certificates;
using System.Text;
using BettingService.API.Utility;
using BettingService.BLL.Utility;
using BettingService.DAL.Models.Settings;
using BettingService.Protos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
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

    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("X-Pagination");
            });
        });
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
        var grpcSettings = configuration.GetSection("GrpcSettings").Get<GrpcSettings>()!;
        var certPath = Path.Combine(Directory.GetCurrentDirectory(), "Properties", "aspnetapp.crt");
        var certificate = new X509Certificate2(certPath);

        services.AddGrpcClient<UserGrpcService.UserGrpcServiceClient>(o =>
            {
                o.Address = new Uri(grpcSettings.UserServiceConnection);
            })
            .ConfigureChannel(o =>
            {
                o.HttpHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        (message, cert, chain, errors) => cert?.Issuer == certificate.Issuer,
                };
            });

        services.AddGrpcClient<SportDataService.SportDataServiceClient>(o =>
            {
                o.Address = new Uri(grpcSettings.SportDataServiceConnection);
            })
            .ConfigureChannel(o =>
            {
                o.HttpHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        (message, cert, chain, errors) => cert?.Issuer == certificate.Issuer,
                };
            });
    }

    public static void ConfigureApiBehaviorOptions(this IServiceCollection services) =>
        services.Configure<ApiBehaviorOptions>(opt => { opt.SuppressModelStateInvalidFilter = true; });
}