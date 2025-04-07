namespace SportDataService.Infrastructure.Services.DataCollection;

public static class JsonNodeNames
{
    // top level nodes
    public const string Sports = "sports";
    public const string CustomFactors = "customFactors";
    public const string Events = "events";

    // object fields
    public const string Id = "id";
    public const string ParentId = "parentId";
    public const string SportId = "sportId";
    public const string RootKind = "rootKind";
    public const string Team1 = "team1";
    public const string Team2 = "team2";
    public const string Team1Id = "team2Id";
    public const string Team2Id = "team2Id";
    public const string Factors = "factors";
    public const string StartTime = "startTime";
    public const string TournamentName = "name";
    public const string SpecialTableId = "specialTableId";

    // factor fields
    public const string EventId = "e";
    public const string FactorId = "f";
    public const string Value = "v";

    // values
    public const string CyberSportIdValue = "29086";
    public const string MatchRootKindValue = "1";
}