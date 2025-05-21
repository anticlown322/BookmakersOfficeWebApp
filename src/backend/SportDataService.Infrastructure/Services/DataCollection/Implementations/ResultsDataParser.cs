using Newtonsoft.Json.Linq;
using SportDataService.Domain.Models.Common;
using SportDataService.Domain.Models.Results;
using SportDataService.Infrastructure.Services.DataCollection.Abstractions;
using SportDataService.Infrastructure.Services.DataCollection.Helpers;

namespace SportDataService.Infrastructure.Services.DataCollection.Implementations;

public class ResultsDataParser : IResultsDataParser
{
    public List<TournamentResult> ParseTournamentResultsData(JToken resultsRawData)
    {
        var tournamentResults = new List<TournamentResult>();

        var tournaments = resultsRawData.GetList(ResultNodeNames.Tournaments)
            .Where(t => t.GetStringValue(ResultNodeNames.SportId) == ResultNodeNames.CyberSportIdValue);

        foreach (var tournament in tournaments)
        {
            var tournamentResult = new TournamentResult
            {
                TournamentId = tournament.GetStringValue(ResultNodeNames.TournamentId),
                TournamentName = tournament.GetStringValue(ResultNodeNames.TournamentName),
            };

            tournamentResults.Add(tournamentResult);
        }

        return tournamentResults;
    }

    public List<MatchResult> ParseMatchResultsData(JToken resultsRawData)
    {
        var matchResults = new List<MatchResult>();
        var matchEventResults = new List<MatchEventResult>();
        var rawMatchesAndEvents = resultsRawData.GetList(ResultNodeNames.MatchesAndEvents);
        var rawMatchOrEventScores = resultsRawData.GetList(ResultNodeNames.MatchOrEventScores);

        var scoresDict = rawMatchOrEventScores
            .ToDictionary(
                score => score[ResultNodeNames.ResultId]?.ToString(),
                score => score);

        foreach (var rawMatchOrEvent in rawMatchesAndEvents)
        {
            var matchOrEventId = rawMatchOrEvent.GetStringValue(ResultNodeNames.MatchOrEventId);

            if (string.IsNullOrEmpty(matchOrEventId) ||
                !scoresDict.TryGetValue(matchOrEventId, out var rawMatchOrEventScore))
            {
                continue;
            }

            var kindId = rawMatchOrEvent.GetStringValue(ResultNodeNames.MatchOrEventKindId);
            switch (kindId)
            {
                case ResultNodeNames.MatchKindIdValue:
                {
                    var matchResult = new MatchResult
                    {
                        MatchResultId = matchOrEventId,
                        TournamentId = rawMatchOrEvent.GetStringValue(ResultNodeNames.MatchTournamentId),
                        MatchName = rawMatchOrEvent.GetStringValue(ResultNodeNames.MatchOrEventName),
                        Team1 = new Team
                        {
                            TeamId = rawMatchOrEvent.GetStringValue(ResultNodeNames.MatchTeam1Id),
                            Name = rawMatchOrEvent.GetStringValue(ResultNodeNames.MatchTeam1Name),
                        },
                        Team2 = new Team
                        {
                            TeamId = rawMatchOrEvent.GetStringValue(ResultNodeNames.MatchTeam2Id),
                            Name = rawMatchOrEvent.GetStringValue(ResultNodeNames.MatchTeam2Name),
                        },
                        ResultTime = ParsingUtils.ParseDateTime(
                            rawMatchOrEvent.GetStringValue(ResultNodeNames.MatchStartTime)),
                        Status = rawMatchOrEvent[ResultNodeNames.MatchOrEventStatus].ToObject<ResultStatus>(),
                    };

                    ProcessMatchSubScores(matchResult, rawMatchOrEventScore);

                    matchResults.Add(matchResult);

                    break;
                }

                case ResultNodeNames.EventKindIdValue:
                {
                    var matchEventResult = new MatchEventResult
                    {
                        MatchEventResultId = matchOrEventId,
                        ParentMatchResultId = rawMatchOrEvent.GetStringValue(ResultNodeNames.EventMatchId),
                        EventName = rawMatchOrEvent.GetStringValue(ResultNodeNames.MatchOrEventName),
                        Status = rawMatchOrEvent[ResultNodeNames.MatchOrEventStatus].ToObject<ResultStatus>(),
                    };

                    ProcessMatchEventSubScores(matchEventResult, rawMatchOrEventScore);

                    matchEventResults.Add(matchEventResult);

                    break;
                }

                default:
                    continue;
            }
        }

        LinkMatchResultsToEventResults(matchResults, matchEventResults);

        return matchResults;
    }

    public void LinkMatchResultsToTournamentResults(
        List<TournamentResult> tournamentResults,
        List<MatchResult> matchResults)
    {
        var tournamentDict = tournamentResults.ToDictionary(t => t.TournamentId);

        foreach (var matchResult in matchResults)
        {
            if (tournamentDict.TryGetValue(matchResult.TournamentId, out var tournament))
            {
                tournament.Matches.Add(matchResult);
            }
        }
    }

    private void ProcessMatchSubScores(MatchResult matchResult, JToken matchScores)
    {
        matchResult.Team1TotalScore = matchScores[ResultNodeNames.Team1TotalScore]?.Value<int>() ?? 0;
        matchResult.Team2TotalScore = matchScores[ResultNodeNames.Team2TotalScore]?.Value<int>() ?? 0;

        var matchSubscores = matchScores.GetList(ResultNodeNames.SubScores);
        foreach (var matchSubScore in matchSubscores)
        {
            var subscore = new SubScore
            {
                SubscorePosition = matchSubScore[ResultNodeNames.SubScorePosition]?.Value<int>() ?? -1,
                Team1Score = matchSubScore[ResultNodeNames.Team1SubScore]?.Value<int>() ?? 0,
                Team2Score = matchSubScore[ResultNodeNames.Team2SubScore]?.Value<int>() ?? 0,
                Title = matchSubScore.GetStringValue(ResultNodeNames.SubScoreName),
            };

            matchResult.SubScores.Add(subscore);
        }
    }

    private void ProcessMatchEventSubScores(MatchEventResult matchEventResult, JToken eventScores)
    {
        matchEventResult.Team1TotalScore = eventScores[ResultNodeNames.Team1TotalScore]?.Value<int>() ?? 0;
        matchEventResult.Team2TotalScore = eventScores[ResultNodeNames.Team2TotalScore]?.Value<int>() ?? 0;

        var matchEventSubscores = eventScores.GetList(ResultNodeNames.SubScores);
        foreach (var matchSubScore in matchEventSubscores)
        {
            var subscore = new SubScore
            {
                SubscorePosition = matchSubScore[ResultNodeNames.SubScorePosition]?.Value<int>() ?? -1,
                Team1Score = matchSubScore[ResultNodeNames.Team1SubScore]?.Value<int>() ?? 0,
                Team2Score = matchSubScore[ResultNodeNames.Team2SubScore]?.Value<int>() ?? 0,
                Title = matchSubScore.GetStringValue(ResultNodeNames.SubScoreName),
            };

            matchEventResult.SubScores.Add(subscore);
        }
    }

    private void LinkMatchResultsToEventResults(
        List<MatchResult> matchResults,
        List<MatchEventResult> matchEventResults)
    {
        var matchDictionary = matchResults.ToDictionary(t => t.MatchResultId);

        foreach (var matchEventResult in matchEventResults)
        {
            if (matchDictionary.TryGetValue(matchEventResult.ParentMatchResultId, out var matchResult))
            {
                matchResult.EventResults.Add(matchEventResult);
            }
        }
    }
}