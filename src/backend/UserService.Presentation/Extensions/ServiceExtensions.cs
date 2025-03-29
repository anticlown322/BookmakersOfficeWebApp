using System.Net;
using System.Net.Mail;
using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NLog;
using UserService.Application.Contracts;
using UserService.Application.Contracts.Services;
using UserService.Application.Contracts.UseCases;
using UserService.Application.Contracts.UseCases.Account;
using UserService.Application.Contracts.UseCases.Authentication;
using UserService.Application.Contracts.UseCases.Balance;
using UserService.Application.Contracts.UseCases.User;
using UserService.Application.DTO;
using UserService.Application.DTO.Authentication;
using UserService.Application.DTO.MappingProfiles;
using UserService.Application.UseCases;
using UserService.Application.UseCases.Account;
using UserService.Application.UseCases.Authentication;
using UserService.Application.UseCases.Balance;
using UserService.Application.UseCases.User;
using UserService.Application.Validation.Validators;
using UserService.Domain.Models;
using UserService.Domain.RepositoryContracts;
using UserService.Infrastructure;
using UserService.Infrastructure.Logs;
using UserService.Infrastructure.Repository;
using UserService.Infrastructure.Repository.Repositories;
using UserService.Infrastructure.Services;

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
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
    }

    public static void ConfigureLoggerService(this IServiceCollection services) =>
        services.AddSingleton<ILoggerService, LoggerService>();

    public static void AddTokenService(this IServiceCollection services) =>
        services.AddScoped<ITokenService, TokenService>();

    public static void AddEmailService(this IServiceCollection services)
    {
        services.AddScoped<IEmailService, EmailService>();

        var serviceProvider = services.BuildServiceProvider();
        var emailSettings = serviceProvider.GetRequiredService<IOptions<EmailSettings>>().Value;
        services
            .AddFluentEmail(emailSettings.SenderEmail, emailSettings.SenderName)
            .AddSmtpSender(() => new SmtpClient
            {
                Host = emailSettings.Server,
                Port = emailSettings.Port,
                EnableSsl = emailSettings.UseSsl,
                Credentials = new NetworkCredential(
                    emailSettings.Username,
                    emailSettings.Password),
            });
    }

    public static void AddUserRepository(this IServiceCollection services) =>
        services.AddScoped<IUsersRepository, UserRepository>();

    public static void ConfigureUseCases(this IServiceCollection services)
    {
        // auth
        services.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();
        services.AddScoped<ILoginUseCase, LoginUseCase>();
        services.AddScoped<IRefreshTokenForAuthUseCase, RefreshTokenForAuthUseCase>();
        services.AddScoped<ILogoutUseCase, LogoutUseCase>();

        // users
        services.AddScoped<IGetAllUsersUseCase, GetAllUsersUseCase>();
        services.AddScoped<IGetUserByIdUseCase, GetUserByIdUseCase>();
        services.AddScoped<IGetUserByNameUseCase, GetUserByNameUseCase>();
        services.AddScoped<IDeleteUserUseCase, DeleteUserUseCase>();

        // account
        services.AddScoped<ISendConfirmationEmailUseCase, SendConfirmationEmailUseCase>();
        services.AddScoped<IConfirmEmailUseCase, ConfirmEmailUseCase>();
        services.AddScoped<ISendResetPasswordEmailUseCase, SendResetPasswordEmailUseCase>();
        services.AddScoped<IResetPasswordUseCase, ResetPasswordUseCase>();
        services.AddScoped<IGetUserProfileUseCase, GetUserProfileUseCase>();
        services.AddScoped<IUpdateUserProfileUseCase, UpdateUserProfileUseCase>();

        // balance
        services.AddScoped<IGetUserBalanceUseCase, GetUserBalanceUseCase>();
        services.AddScoped<IDepositToUserBalanceUseCase, DepositToUserBalanceUseCase>();
        services.AddScoped<IWithDrawFromUserBalanceUseCase, WithDrawFromUserBalanceUseCase>();
        services.AddScoped<IGetTransactionHistory, GetTransactionHistoryUseCase>();
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
            cfg.AddProfile<GetUsersMappingProfile>();
            cfg.AddProfile<GetUserProfileMappingProfile>();
            cfg.AddProfile<UpdateUserProfileMappingProfile>();
            cfg.AddProfile<GetTransactionsMappingProfile>();
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

    public static void AddAuthorizationPolicies(this IServiceCollection services) =>
        services.AddAuthorization(options =>
        {
            options.AddPolicy("GamblerOnly", policy =>
                policy.RequireRole("Gambler"));

            options.AddPolicy("ModeratorOnly", policy =>
                policy.RequireRole("Moderator"));

            options.AddPolicy("BookmakerOnly", policy =>
                policy.RequireRole("Bookmaker"));

            options.AddPolicy("AdministratorOnly", policy =>
                policy.RequireRole("Administrator"));

            options.AddPolicy("AdministratorOrGambler", policy =>
                policy.RequireRole("Gambler", "Administrator"));

            options.AddPolicy("AdministratorOrModerator", policy =>
                policy.RequireRole("Administrator", "Moderator"));

            options.AddPolicy("AdministratorOrModeratorOrGambler", policy =>
                policy.RequireRole("Administrator", "Moderator", "Gambler"));

            options.AddPolicy("AllUsers", policy =>
                policy.RequireRole("Gambler", "Moderator", "Bookmaker", "Administrator"));
        });

    public static void AddValidators(this IServiceCollection services)
    {
        services.AddTransient<IValidator<User>, UserValidator>();
        services.AddTransient<IValidator<UserForRegistrationDto>, UserRegistrationDtoValidator>();
        services.AddTransient<IValidator<UserForLoginDto>, UserAuthenticationDtoValidator>();
    }

    public static void ConfigureApiBehaviorOptions(this IServiceCollection services) =>
        services.Configure<ApiBehaviorOptions>(opt => { opt.SuppressModelStateInvalidFilter = true; });
}