using System.IO.Compression;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SportDataService.Application.Contracts.Services;
using SportDataService.Domain.Models.Markets;
using SportDataService.Domain.Models.Settings;
using SportDataService.Domain.Models.Tournaments;

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
            using var response = await httpClient.GetAsync(_settings.Url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            cancellationToken.ThrowIfCancellationRequested();

            await using var responseStream = await response.Content.ReadAsStreamAsync();

            cancellationToken.ThrowIfCancellationRequested();

            await using var decompressedStream = new GZipStream(responseStream, CompressionMode.Decompress);

            using var jsonReader = new StreamReader(decompressedStream);
            using var jsonTextReader = new JsonTextReader(jsonReader);

            cancellationToken.ThrowIfCancellationRequested();

            var answer = await JToken.LoadAsync(jsonTextReader, cancellationToken);

            var tournaments = ParseAllTournaments(answer);
            var matches = ParseMatchesInfo(answer);
            LinkMatchesToTournaments(tournaments, matches);

            return tournaments;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Can't get API response: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching data: {ex.Message}");
            throw;
        }
    }

    private List<Tournament> ParseAllTournaments(JToken apiAnswer)
    {
        var tournaments = new List<Tournament>();

        var sports = apiAnswer.GetList(JsonNodeNames.Sports)
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

    private List<Match> ParseMatchesInfo(JToken apiAnswer)
    {
        var matches = new List<Match>();
        var rawFactors = apiAnswer.GetList(JsonNodeNames.CustomFactors);
        var rawEvents = apiAnswer.GetList(JsonNodeNames.Events);

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
}