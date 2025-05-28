using System.Net;
using System.Net.Mail;
using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Json;
using UserService.Application.Contracts.Services;
using UserService.Application.Contracts.UseCases.Account;
using UserService.Application.Contracts.UseCases.Authentication;
using UserService.Application.Contracts.UseCases.Balance;
using UserService.Application.Contracts.UseCases.User;
using UserService.Application.DTO.Account;
using UserService.Application.DTO.Authentication;
using UserService.Application.DTO.Balance;
using UserService.Application.DTO.MappingProfiles;
using UserService.Application.UseCases.Account;
using UserService.Application.UseCases.Authentication;
using UserService.Application.UseCases.Balance;
using UserService.Application.UseCases.User;
using UserService.Application.Validation.Validators.Account;
using UserService.Application.Validation.Validators.Authentication;
using UserService.Application.Validation.Validators.Balance;
using UserService.Domain.Models;
using UserService.Domain.RepositoryContracts;
using UserService.Infrastructure.Repository;
using UserService.Infrastructure.Repository.Repositories;
using UserService.Infrastructure.Services;
using UserService.Infrastructure.Utility;
using UserService.Presentation.Utility;

namespace UserService.Presentation.Extensions;

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
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
    }

    public static void ConfigureLogging(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .WriteTo.Console()
            .WriteTo.Http(
                requestUri: configuration["Logstash:Uri"],
                queueLimitBytes: null,
                textFormatter: new JsonFormatter())
            .CreateLogger();
    }

    public static void AddTokenService(this IServiceCollection services) =>
        services.AddScoped<ITokenService, TokenService>();

    public static void AddEmailService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IEmailService, EmailService>();

        var emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>()!;
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

    public static void AddDatabaseMigrationService(this IServiceCollection services) =>
        services.AddScoped<IDatabaseMigrationService, DatabaseMigrationService>();

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
        services.AddScoped<IWithdrawFromUserBalanceUseCase, WithdrawFromUserBalanceUseCase>();
        services.AddScoped<IGetTransactionHistory, GetTransactionHistoryUseCase>();
    }

    public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DbConnection");

        services.AddDbContext<RepositoryContext>(opts => opts.UseNpgsql(connectionString));
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
            },
            AppDomain.CurrentDomain.GetAssemblies());
    }

    public static void ConfigureIdentity(this IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole>(o =>
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

    public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()!;

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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                };
            });
    }

    public static void AddAuthorizationPolicies(this IServiceCollection services) =>
        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                AuthorizationPolicies.GamblerOnly,
                policy =>
                    policy.RequireRole(UserRoles.Gambler));

            options.AddPolicy(
                AuthorizationPolicies.ModeratorOnly,
                policy =>
                    policy.RequireRole(UserRoles.Moderator));

            options.AddPolicy(
                AuthorizationPolicies.BookmakerOnly,
                policy =>
                    policy.RequireRole(UserRoles.Bookmaker));

            options.AddPolicy(
                AuthorizationPolicies.AdministratorOnly,
                policy =>
                    policy.RequireRole(UserRoles.Administrator));

            options.AddPolicy(
                AuthorizationPolicies.AdministratorOrGambler,
                policy =>
                    policy.RequireRole(UserRoles.Gambler, UserRoles.Administrator));

            options.AddPolicy(
                AuthorizationPolicies.AdministratorOrModerator,
                policy =>
                    policy.RequireRole(UserRoles.Administrator, UserRoles.Moderator));

            options.AddPolicy(
                AuthorizationPolicies.AdministratorOrModeratorOrGambler,
                policy =>
                    policy.RequireRole(UserRoles.Administrator, UserRoles.Moderator, UserRoles.Gambler));

            options.AddPolicy(
                AuthorizationPolicies.AllUsers,
                policy =>
                    policy.RequireRole(
                        UserRoles.Administrator,
                        UserRoles.Gambler,
                        UserRoles.Moderator,
                        UserRoles.Bookmaker));
        });

    public static void AddValidators(this IServiceCollection services)
    {
        // account
        services.AddTransient<IValidator<PasswordResetDto>, PasswordResetDtoValidator>();
        services.AddTransient<IValidator<UserProfileUpdateDto>, UserProfileUpdateDtoValidator>();

        // auth
        services.AddTransient<IValidator<UserRegistrationDto>, UserRegistrationDtoValidator>();
        services.AddTransient<IValidator<UserLoginDto>, UserLoginDtoValidator>();
        services.AddTransient<IValidator<TokensRefreshDto>, TokensRefreshDtoValidator>();
        services.AddTransient<IValidator<UserLogoutDto>, UserLogoutDtoValidator>();

        // balance
        services.AddTransient<IValidator<DepositRequestDto>, DepositRequestDtoValidator>();
        services.AddTransient<IValidator<WithdrawRequestDto>, WithdrawRequestDtoValidator>();
    }

    public static void ConfigureApiBehaviorOptions(this IServiceCollection services) =>
        services.Configure<ApiBehaviorOptions>(opt => { opt.SuppressModelStateInvalidFilter = true; });
}