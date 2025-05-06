namespace Parser.DataCollection;

public static class ResultNodeNames
{
    // Top level nodes
    public const string Tournaments = "competitions";
    public const string MatchesAndEvents = "events";
    public const string MatchOrEventScores = "eventMiscs";

    // Competition nodes
    public const string TournamentId = "id"; 
    public const string TournamentName = "name"; 
    public const string SportId = "sportId"; 
    
    // Match record nodes
    public const string MatchOrEventId = "id";
    public const string MatchOrEventName = "name";
    public const string MatchOrEventKindId = "kindId";
    public const string MatchTournamentId = "competitionId";
    public const string MatchStartTime = "startTime";
    public const string MatchTeam1Id = "team1Id";
    public const string MatchTeam2Id = "team2Id";    
    public const string MatchTeam1Name = "team1";
    public const string MatchTeam2Name = "team2";
    public const string MatchOrEventStatus = "status";
    public const string EventMatchId = "parentId";
    
    // Result nodes
    public const string ResultId = "id";
    public const string Team1TotalScore = "score1";
    public const string Team2TotalScore = "score2";
    public const string SubScores = "subScores";
    public const string Team1SubScore = "score1";
    public const string Team2SubScore = "score2";
    public const string SubScorePosition = "scoreIndex";
    public const string SubScoreName = "title";
    public const string MatchTimeCounterType = "timerDirection";
    public const string SecondsPassed = "timerSeconds";
    public const string MatchStartedAt = "timerUpdateTimestampMsec";
    
    // Values
    public const string MatchKindIdValue = "1";
    public const string EventKindIdValue = "104000";
    public const string CyberSportIdValue = "29086";
}