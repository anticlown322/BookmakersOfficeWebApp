using Newtonsoft.Json.Linq;
using SportDataService.Domain.Models.Results;

namespace SportDataService.Infrastructure.Services.DataCollection.Abstractions;

public interface IResultsDataParser
{
    List<TournamentResult> ParseTournamentResultsData(JToken resultsRawData);
    List<MatchResult> ParseMatchResultsData(JToken resultsRawData);
    void LinkMatchResultsToTournamentResults(
        List<TournamentResult> tournamentResults,
        List<MatchResult> matchResults);
}