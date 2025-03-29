namespace UserService.Domain.Models;

public class UserBalance
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public decimal CurrentAmount { get; set; }
    public DateTime LastUpdated { get; set; }
    public User User { get; set; }
    public ICollection<BalanceTransaction> Transactions { get; set; } = new List<BalanceTransaction>();
}