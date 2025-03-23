using System.Text;
using Application.DTO.User;
using Application.UseCases.Authentication;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NLog;
using UserService.Application.Contracts;
using UserService.Application.Contracts.UseCaseContracts;
using UserService.Application.DTO;
using UserService.Application.DTO.MappingProfiles;
using UserService.Application.UseCases;
using UserService.Application.Validation.Validators;
using UserService.Domain.Models;
using UserService.Domain.RepositoryContracts;
using UserService.Infrastructure;
using UserService.Infrastructure.AppSettings;
using UserService.Infrastructure.Logs;
using UserService.Infrastructure.Repository;

namespace UserService.Presentation.Extensions;

public static class ServiceExtensions
{
    public static IConfigurationBuilder AddSecretsYaml(this IConfigurationBuilder configurationBuilder)
    {
        var path = Directory.GetCurrentDirectory() + @"\Properties";

        return configurationBuilder
            .SetBasePath(path)
            .AddYamlFile("secrets.yaml", optional: true, reloadOnChange: true);
    }

    public static void ConfigureNLog(this IServiceCollection services)
    {
        var configFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"Properties\nlog.config");

        if (!File.Exists(configFilePath))
        {
            throw new FileNotFoundException($"NLog configuration file not found: {configFilePath}");
        }

        LogManager.Setup().LoadConfigurationFromFile(configFilePath);
    }

    public static void AddAppSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseSettings>(configuration.GetSection("DatabaseSettings"));
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
    }

    public static void ConfigureLoggerService(this IServiceCollection services) =>
        services.AddSingleton<ILoggerManager, LoggerManager>();

    public static void ConfigureRepositoryManager(this IServiceCollection services) =>
        services.AddScoped<IRepositoryManager, RepositoryManager>();

    public static void ConfigureAuthenticationManager(this IServiceCollection services) =>
        services.AddScoped<IAuthenticationManager, AuthenticationManager>();

    public static void ConfigureUseCases(this IServiceCollection services)
    {
        services.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();
        services.AddScoped<ICreateTokenForAuthUseCase, CreateTokenForAuthUseCase>();
        services.AddScoped<IRefreshTokenForAuthUseCase, RefreshTokenForAuthUseCase>();
    }

    public static void ConfigureSqlContext(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var databaseSettings = serviceProvider.GetRequiredService<IOptions<DatabaseSettings>>().Value;

        services.AddDbContext<RepositoryContext>(opts =>
            opts.UseNpgsql(databaseSettings.ConnectionString));
    }

    public static void ConfigureAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(
            cfg =>
        {
            cfg.AddProfile<RegisterUserMappingProfile>();
        }, AppDomain.CurrentDomain.GetAssemblies());
    }

    public static void ConfigureIdentity(this IServiceCollection services)
    {
        var builder = services.AddIdentity<User, IdentityRole>(o =>
            {
                o.Password.RequireDigit = true;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 10;
                o.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<RepositoryContext>()
            .AddDefaultTokenProviders();
    }

    public static void ConfigureJwt(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var jwtSettings = serviceProvider.GetRequiredService<IOptions<JwtSettings>>().Value;
        var secretKey = jwtSettings.SecretKey;

        services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
                };
            });
    }

    public static void AddValidators(this IServiceCollection services)
    {
        services.AddTransient<IValidator<User>, UserValidator>();
        services.AddTransient<IValidator<UserForRegistrationDto>, UserRegistrationDtoValidator>();
        services.AddTransient<IValidator<UserForAuthenticationDto>, UserAuthenticationDtoValidator>();
    }

    public static void ConfigureApiBehaviorOptions(this IServiceCollection services) =>
        services.Configure<ApiBehaviorOptions>(opt => { opt.SuppressModelStateInvalidFilter = true; });
}