using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using UserService.Domain.Models;
using UserService.Infrastructure.Repository;
using UserService.Infrastructure.Utility;

namespace UserService.Tests.IntegrationTests.Fixtures;

public class DatabaseFixture : IAsyncLifetime
{
    private PostgreSqlContainer _postgresContainer;
    public string ConnectionString { get; private set; }
    public IServiceProvider ServiceProvider { get; private set; }
    public UserManager<User> UserManager { get; private set; }
    public RepositoryContext DbContext { get; private set; }
    private bool _disposed = false;

    public async Task InitializeAsync()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(DatabaseFixture));
        }
        
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:15")
            .WithDatabase("test_db")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithCleanUp(true)
            .Build();

        await _postgresContainer.StartAsync();
        ConnectionString = _postgresContainer.GetConnectionString();

        var services = new ServiceCollection();
        
        services.AddLogging();
        services.AddHttpContextAccessor();
        services.AddDbContext<RepositoryContext>(options =>
            options.UseNpgsql(ConnectionString));

        services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 10;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<RepositoryContext>()
            .AddDefaultTokenProviders();

        ServiceProvider = services.BuildServiceProvider();

        DbContext = ServiceProvider.GetRequiredService<RepositoryContext>();

        await DbContext.Database.MigrateAsync();

        await InitializeRolesAsync();
        
        UserManager = ServiceProvider.GetRequiredService<UserManager<User>>();
    }

    public async Task DisposeAsync()
    {
        if (!_disposed)
        {
            _disposed = true;
            
            if (DbContext != null)
                await DbContext.Database.EnsureDeletedAsync();

            await _postgresContainer.DisposeAsync();
        }
    }
    
    private async Task InitializeRolesAsync()
    {
        var roleManager = ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        
        var roles = new[]
        {
            UserRoles.Guest,
            UserRoles.Gambler,
            UserRoles.Moderator,
            UserRoles.Bookmaker,
            UserRoles.Administrator,
            UserRoles.Banned
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}