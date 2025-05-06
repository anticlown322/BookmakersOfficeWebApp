using System.IO.Compression;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Parser.DataCollection;
using SportDataService.Application.Contracts.Services;
using SportDataService.Domain.Models.Common;
using SportDataService.Domain.Models.Prematch;
using SportDataService.Domain.Models.Prematch.Markets;
using SportDataService.Domain.Models.Results;
using SportDataService.Domain.Models.Settings;

namespace SportDataService.Infrastructure.Services.DataCollection;

public class DataCollectionService(
    IOptions<DataCollectionServiceSettings> settings)
    : IDataCollectionService
{
    private readonly DataCollectionServiceSettings _settings = settings.Value;

    public async Task<List<Tournament>> GetTournamentsInfoAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var httpClient = new HttpClient();
            using var response = await httpClient.GetAsync(
                _settings.MarketsUrl,
                HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            cancellationToken.ThrowIfCancellationRequested();

            await using var responseStream = await response.Content.ReadAsStreamAsync();

            cancellationToken.ThrowIfCancellationRequested();

            await using var decompressedStream = new GZipStream(responseStream, CompressionMode.Decompress);

            using var jsonReader = new StreamReader(decompressedStream);
            using var jsonTextReader = new JsonTextReader(jsonReader);

            cancellationToken.ThrowIfCancellationRequested();

            var rawSportData = await JToken.LoadAsync(jsonTextReader, cancellationToken);

            var tournaments = ParseAllTournaments(rawSportData);
            var matches = ParseMatchesInfo(rawSportData);
            LinkMatchesToTournaments(tournaments, matches);

            return tournaments;
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("Can't get API response");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching data: {ex.Message}");
            throw;
        }
    }

    public async Task<List<TournamentResult>> GetTournamentsResultsInfoAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var httpClient = new HttpClient();
            var currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            var fullUrl = _settings.ResultsUrl + currentDate;
            using var response = await httpClient.GetAsync(
                fullUrl,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);
            response.EnsureSuccessStatusCode();

            cancellationToken.ThrowIfCancellationRequested();

            await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            await using var decompressedStream = new GZipStream(responseStream, CompressionMode.Decompress);

            using var jsonReader = new StreamReader(decompressedStream);
            using var jsonTextReader = new JsonTextReader(jsonReader);

            cancellationToken.ThrowIfCancellationRequested();

            var rawTournamentsResultsData = await JToken.LoadAsync(jsonTextReader, cancellationToken);
            var tournamentsResults = ParseAllTournamentResults(rawTournamentsResultsData);
            var matchesResults = ParseMatchResults(rawTournamentsResultsData);

            LinkMatchesToTournamentsResults(tournamentsResults, matchesResults);

            return tournamentsResults;
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("Can't get API response");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching data: {ex.Message}");
            throw;
        }
    }

    private List<Tournament> ParseAllTournaments(JToken sportData)
    {
        var tournaments = new List<Tournament>();

        var sports = sportData.GetList(JsonNodeNames.Sports)
            .Where(t => t.GetStringValue(JsonNodeNames.ParentId) == JsonNodeNames.CyberSportIdValue);

        foreach (var sport in sports)
        {
            // SpecialTableId is only for tournament results, not matches
            var isMatch = sport.GetStringValue(JsonNodeNames.SpecialTableId) == null;
            if (!isMatch)
            {
                continue;
            }

            var tournament = new Tournament
            {
                TournamentId = sport.GetStringValue(JsonNodeNames.Id),
                Name = sport.GetStringValue(JsonNodeNames.TournamentName),
            };

            tournaments.Add(tournament);
        }

        return tournaments;
    }

    private List<Match> ParseMatchesInfo(JToken sportData)
    {
        var matches = new List<Match>();
        var rawFactors = sportData.GetList(JsonNodeNames.CustomFactors);
        var rawEvents = sportData.GetList(JsonNodeNames.Events);

        var factorsByEventId = rawFactors
            .GroupBy(f => f.GetStringValue(JsonNodeNames.EventId))
            .ToDictionary(
                g => g.Key,
                g => g.SelectMany(f => f.GetList(JsonNodeNames.Factors)).ToList(),
                StringComparer.OrdinalIgnoreCase);

        foreach (var rawEvent in rawEvents)
        {
            var matchId = rawEvent.GetStringValue(JsonNodeNames.Id);
            if (string.IsNullOrEmpty(matchId) ||
                !factorsByEventId.TryGetValue(matchId, out var factors))
            {
                continue;
            }

            if (rawEvent.GetStringValue(JsonNodeNames.RootKind) != JsonNodeNames.MatchRootKindValue)
            {
                continue;
            }

            var match = new Match
            {
                MatchId = matchId,
                TournamentId = rawEvent.GetStringValue(JsonNodeNames.SportId),
                StartTime = ParseDateTime(rawEvent.GetStringValue(JsonNodeNames.StartTime)),
                Opponent1 = new Team
                {
                    TeamId = rawEvent.GetStringValue(JsonNodeNames.Team1Id),
                    Name = rawEvent.GetStringValue(JsonNodeNames.Team1),
                },
                Opponent2 = new Team
                {
                    TeamId = rawEvent.GetStringValue(JsonNodeNames.Team2Id),
                    Name = rawEvent.GetStringValue(JsonNodeNames.Team2),
                },
            };

            ProcessMarkets(match, factors);
            matches.Add(match);
        }

        return matches;
    }

    private void ProcessMarkets(Match match, List<JToken> factors)
    {
        foreach (var factor in factors)
        {
            var factorId = factor.GetStringValue(JsonNodeNames.FactorId);
            var value = factor.GetStringValue(JsonNodeNames.Value);
            var marketValue = new MarketValue { Value = value };

            switch (factorId)
            {
                // main line
                case MarketIds.Opponent1Win:
                    match.MainLine.Opponent1Win = marketValue;
                    break;
                case MarketIds.Opponent2Win:
                    match.MainLine.Opponent2Win = marketValue;
                    break;
                case MarketIds.Draw:
                    match.MainLine.Draw = marketValue;
                    break;
                case MarketIds.Opponent1WinOrDraw:
                    match.MainLine.Opponent1WinOrDraw = marketValue;
                    break;
                case MarketIds.Opponent2WinOrDraw:
                    match.MainLine.Opponent2WinOrDraw = marketValue;
                    break;

                // kills line
                case MarketIds.Opponent1KillsMain:
                    match.KillsLine.Opponent1KillsMain = marketValue;
                    break;
                case MarketIds.Opponent2KillsMain:
                    match.KillsLine.Opponent2KillsMain = marketValue;
                    break;
                case MarketIds.TotalKillsUnder:
                    match.KillsLine.TotalKillsUnder = marketValue;
                    break;
                case MarketIds.TotalKillsOver:
                    match.KillsLine.TotalKillsOver = marketValue;
                    break;
                case MarketIds.Opponent1KillsHandicap:
                    match.KillsLine.Opponent1KillsHandicap = marketValue;
                    break;
                case MarketIds.Opponent2KillsHandicap:
                    match.KillsLine.Opponent2KillsHandicap = marketValue;
                    break;

                // maps line
                case MarketIds.Map1HandicapOpponent1:
                    match.MapsLine.Map1HandicapOpponent1 = marketValue;
                    break;
                case MarketIds.Map1HandicapOpponent2:
                    match.MapsLine.Map1HandicapOpponent2 = marketValue;
                    break;
                case MarketIds.Map2HandicapOpponent1:
                    match.MapsLine.Map2HandicapOpponent1 = marketValue;
                    break;
                case MarketIds.Map2HandicapOpponent2:
                    match.MapsLine.Map2HandicapOpponent2 = marketValue;
                    break;

                // other markets, special line
                case MarketIds.EitherOpponent1OrOpponent2:
                    match.SpecialLine.EitherOpponent1OrOpponent2 = marketValue;
                    break;
            }
        }
    }

    private void LinkMatchesToTournaments(List<Tournament> tournaments, List<Match> matches)
    {
        var tournamentDict = tournaments.ToDictionary(t => t.TournamentId);

        foreach (var match in matches)
        {
            if (tournamentDict.TryGetValue(match.TournamentId, out var tournament))
            {
                tournament.Matches.Add(match);
            }
        }
    }

    private List<TournamentResult> ParseAllTournamentResults(JToken resultsRawData)
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

    private List<MatchResult> ParseMatchResults(JToken resultsRawData)
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
                        ResultTime = ParseDateTime(rawMatchOrEvent.GetStringValue(ResultNodeNames.MatchStartTime)),
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
                Title = matchSubScore.GetStringValue(ResultNodeNames.SubScoreName)
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
                Title = matchSubScore.GetStringValue(ResultNodeNames.SubScoreName)
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

    private void LinkMatchesToTournamentsResults(
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

    private DateTime? ParseDateTime(JToken timeToken)
    {
        if (string.IsNullOrWhiteSpace(timeToken.ToString()))
        {
            return null;
        }

        if (long.TryParse(timeToken.ToString(), out var unixTimestamp))
        {
            // add 3 hours for UTC + 3 time (Moscow / Minsk)
            var dateTime = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp)
                .UtcDateTime
                .AddHours(3);

            return dateTime;
        }

        return null;
    }
}