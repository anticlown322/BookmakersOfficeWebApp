namespace BettingService.DAL.Models.Entities;

public class Payout
{
    public Guid Id { get; set; }          
    public Guid BetId { get; set; }       
    public decimal Amount { get; set; }    
    public PayoutStatus Status { get; set; } 
    public DateTime ProcessedAt { get; set; } 
    public string? ErrorReason { get; set; } 
}