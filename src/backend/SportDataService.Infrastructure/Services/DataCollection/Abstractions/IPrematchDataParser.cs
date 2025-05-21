using Newtonsoft.Json.Linq;
using SportDataService.Domain.Models.Prematch;

namespace SportDataService.Infrastructure.Services.DataCollection.Abstractions;

public interface IPrematchDataParser
{
    List<Tournament> ParseTournamentsPrematchData(JToken sportData);
    List<Match> ParseMatchesPrematchData(JToken sportData);
    void LinkMatchesToTournaments(List<Tournament> tournaments, List<Match> matches);
}