namespace UserService.Domain.Models;

public class BalanceTransaction
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string OperationType { get; set; }
    public string? Comment { get; set; }
    public User User { get; set; }
}