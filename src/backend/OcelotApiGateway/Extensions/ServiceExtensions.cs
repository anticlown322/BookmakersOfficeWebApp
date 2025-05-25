using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace OcelotApiGateway.Extensions;

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

    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
    }
    
    public static void ConfigureAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var settingsSection = configuration.GetSection("JwtSettings");

        var secret = settingsSection.GetValue<string>("SecretKey");
        var issuer = settingsSection.GetValue<string>("Issuer");
        var audience = settingsSection.GetValue<string>("Audience");

        var key = Encoding.ASCII.GetBytes(secret!);
        
        services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer();

        services.PostConfigure<JwtBearerOptions>(
            JwtBearerDefaults.AuthenticationScheme,
            options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero,
                };
            });
    }
}