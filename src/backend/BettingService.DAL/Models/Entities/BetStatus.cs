namespace BettingService.DAL.Models.Entities;

public enum BetStatus
{
    Validating,
    Rejected,
    Pending,
    Active,
    Won,
    Lost,
    Cancelled,
    Refunded,
}