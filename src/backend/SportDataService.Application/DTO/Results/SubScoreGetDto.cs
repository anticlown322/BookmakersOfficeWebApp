namespace SportDataService.Application.DTO.Results;

public class SubScoreGetDto
{
    public int SubscorePosition { get; set; }
    public string? Title { get; set; }
    public int Team1Score { get; set; }
    public int Team2Score { get; set; }
}