namespace SportDataService.Domain.Models.Results;

public class SubScore
{
    public int SubscorePosition { get; set; }
    public string? Title { get; set; }
    public int Team1Score { get; set; }
    public int Team2Score { get; set; }
}