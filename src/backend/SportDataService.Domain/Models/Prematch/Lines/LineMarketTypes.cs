namespace SportDataService.Domain.Models.Prematch.Lines;

public static class LineMarketTypes
{
    // MainLine
    public const string Opponent1Win = "Opponent1Win";
    public const string Opponent2Win = "Opponent2Win";
    public const string Draw = "Draw";
    public const string Opponent1WinOrDraw = "Opponent1WinOrDraw";
    public const string Opponent2WinOrDraw = "Opponent2WinOrDraw";

    // KillsLine
    public const string Opponent1KillsMain = "Opponent1KillsMain";
    public const string Opponent2KillsMain = "Opponent2KillsMain";
    public const string TotalKillsUnder = "TotalKillsUnder";
    public const string TotalKillsOver = "TotalKillsOver";
    public const string Opponent1KillsHandicap = "Opponent1KillsHandicap";
    public const string Opponent2KillsHandicap = "Opponent2KillsHandicap";

    // MapsLine
    public const string Map1HandicapOpponent1 = "Map1HandicapOpponent1";
    public const string Map1HandicapOpponent2 = "Map1HandicapOpponent2";
    public const string Map2HandicapOpponent1 = "Map2HandicapOpponent1";
    public const string Map2HandicapOpponent2 = "Map2HandicapOpponent2";

    // SpecialLine
    public const string EitherOpponent1OrOpponent2 = "EitherOpponent1OrOpponent2";
}