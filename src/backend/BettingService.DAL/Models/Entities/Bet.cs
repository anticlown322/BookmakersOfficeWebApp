namespace BettingService.DAL.Models.Entities;

public class Bet
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string MatchId { get; set; }

    public BetLineType LineType { get; set; }
    public string MarketSelection { get; set; }

    public decimal Amount { get; set; }
    public decimal Odds { get; set; }

    public BetStatus Status { get; set; }
    public string? RejectionReason { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? SettledAt { get; set; }
    public DateTime? CancelledAt { get; set; }
}