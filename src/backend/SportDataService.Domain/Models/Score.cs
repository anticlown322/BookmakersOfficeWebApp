namespace SportDataService.Domain.Models;

public class Score
{
    public int Home { get; set; }
    public int Away { get; set; }

    public override string ToString() => $"{Home}:{Away}";
    public bool IsDraw => Home == Away;
}