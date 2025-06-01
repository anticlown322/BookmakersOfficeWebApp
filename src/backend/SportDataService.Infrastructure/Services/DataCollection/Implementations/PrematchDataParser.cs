using Newtonsoft.Json.Linq;
using SportDataService.Domain.Models.Common;
using SportDataService.Domain.Models.Prematch;
using SportDataService.Domain.Models.Prematch.Markets;
using SportDataService.Infrastructure.Services.DataCollection.Abstractions;
using SportDataService.Infrastructure.Services.DataCollection.Helpers;

namespace SportDataService.Infrastructure.Services.DataCollection.Implementations;

public class PrematchDataParser : IPrematchDataParser
{
    public List<Tournament> ParseTournamentsPrematchData(JToken sportData)
    {
        var tournaments = new List<Tournament>();

        var sports = sportData.GetList(PrematchNodeNames.Sports)
            .Where(t => t.GetStringValue(PrematchNodeNames.ParentId) == PrematchNodeNames.CyberSportIdValue);

        foreach (var sport in sports)
        {
            // SpecialTableId is only for tournament results, not matches
            var isMatch = sport.GetStringValue(PrematchNodeNames.SpecialTableId) == null;
            if (!isMatch)
            {
                continue;
            }

            var tournament = new Tournament
            {
                TournamentId = sport.GetStringValue(PrematchNodeNames.Id),
                Name = sport.GetStringValue(PrematchNodeNames.TournamentName),
            };

            tournaments.Add(tournament);
        }

        return tournaments;
    }

    public List<Match> ParseMatchesPrematchData(JToken sportData)
    {
        var matches = new List<Match>();
        var rawFactors = sportData.GetList(PrematchNodeNames.CustomFactors);
        var rawEvents = sportData.GetList(PrematchNodeNames.Events);

        var factorsByEventId = rawFactors
            .GroupBy(f => f.GetStringValue(PrematchNodeNames.EventId))
            .ToDictionary(
                g => g.Key,
                g => g.SelectMany(f => f.GetList(PrematchNodeNames.Factors)).ToList(),
                StringComparer.OrdinalIgnoreCase);

        foreach (var rawEvent in rawEvents)
        {
            var matchId = rawEvent.GetStringValue(PrematchNodeNames.Id);
            if (string.IsNullOrEmpty(matchId) ||
                !factorsByEventId.TryGetValue(matchId, out var factors))
            {
                continue;
            }

            if (rawEvent.GetStringValue(PrematchNodeNames.RootKind) != PrematchNodeNames.MatchRootKindValue)
            {
                continue;
            }

            var match = new Match
            {
                MatchId = matchId,
                TournamentId = rawEvent.GetStringValue(PrematchNodeNames.SportId),
                StartTime = ParsingUtils.ParseDateTime(rawEvent.GetStringValue(PrematchNodeNames.StartTime)),
                Opponent1 = new Team
                {
                    TeamId = rawEvent.GetStringValue(PrematchNodeNames.Team1Id),
                    Name = rawEvent.GetStringValue(PrematchNodeNames.Team1),
                },
                Opponent2 = new Team
                {
                    TeamId = rawEvent.GetStringValue(PrematchNodeNames.Team2Id),
                    Name = rawEvent.GetStringValue(PrematchNodeNames.Team2),
                },
            };

            ProcessMarkets(match, factors);
            matches.Add(match);
        }

        return matches;
    }

    public void LinkMatchesToTournaments(List<Tournament> tournaments, List<Match> matches)
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

    private void ProcessMarkets(Match match, List<JToken> factors)
    {
        foreach (var factor in factors)
        {
            var factorId = factor.GetStringValue(PrematchNodeNames.FactorId);
            var value = factor.GetStringValue(PrematchNodeNames.Value);
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
}