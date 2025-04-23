using System.Reflection;
using BettingService.BLL.Contracts.Services;
using BettingService.BLL.DTO.Bet;
using BettingService.BLL.DTO.MappingProfiles;
using BettingService.BLL.DTO.Payout;
using BettingService.BLL.Services;
using BettingService.BLL.Validation.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BettingService.BLL;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services)
    {
        services.AddSingleton<ILoggerService, LoggerService>();
        services.AddScoped<IDatabaseMigrationService, DatabaseMigrationService>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddAutoMapper(
            cfg =>
            {
                cfg.AddProfile<GetBetMappingProfile>();
                cfg.AddProfile<GetPayoutMappingProfile>();
            },
            AppDomain.CurrentDomain.GetAssemblies());

        services.AddTransient<IValidator<CreatePayoutDto>, CreatePayoutDtoValidator>();
        services.AddTransient<IValidator<PlaceBetDto>, PlaceBetDtoValidator>();

        return services;
    }
}