namespace SportDataService.Application.DTO.Match;

public sealed class ScoreGetDto
{
    public int Home { get; set; }
    public int Away { get; set; }
    public bool IsDraw { get; set; }
    public string Display => $"{Home}:{Away}";
}