namespace SportDataService.Domain.Models.Markets;

public static class MarketIds
{
    // main line
    public const string Opponent1Win = "921";
    public const string Draw = "922";
    public const string Opponent2Win = "923";
    public const string Opponent1WinOrDraw = "924";
    public const string Opponent2WinOrDraw = "925";

    // kills line
    public const string Opponent1KillsMain = "927";
    public const string Opponent2KillsMain = "928";
    public const string TotalKillsUnder = "930";
    public const string TotalKillsOver = "931";
    public const string Opponent1KillsHandicap = "989";
    public const string Opponent2KillsHandicap = "991";

    // special line
    public const string EitherOpponent1OrOpponent2 = "1571";

    // maps line
    public const string Map1HandicapOpponent1 = "3262";
    public const string Map1HandicapOpponent2 = "3263";
    public const string Map2HandicapOpponent1 = "3265";
    public const string Map2HandicapOpponent2 = "3266";
}