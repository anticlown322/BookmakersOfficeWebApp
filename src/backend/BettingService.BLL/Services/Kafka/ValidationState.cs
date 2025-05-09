using BettingService.DAL.Models.Kafka.BetValidation;

namespace BettingService.BLL.Services.Kafka;

internal class ValidationState
{
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public UserValidationResult? UserValidation { get; set; }
    public SportValidationResult? SportValidation { get; set; }
}