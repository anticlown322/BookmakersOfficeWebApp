using SportDataService.Application.Contracts.Services;
using SportDataService.Domain.Models.Prematch;
using SportDataService.Domain.Models.Results;
using SportDataService.Infrastructure.Services.DataCollection.Abstractions;

namespace SportDataService.Infrastructure.Services.DataCollection;

public class DataCollectionService : IDataCollectionService
{
    private readonly IApiDataService _apiDataService;
    private readonly IPrematchDataParser _prematchParser;
    private readonly IResultsDataParser _resultsParser;

    public DataCollectionService(
        IApiDataService apiDataService,
        IPrematchDataParser prematchParser,
        IResultsDataParser resultsParser)
    {
        _apiDataService = apiDataService;
        _prematchParser = prematchParser;
        _resultsParser = resultsParser;
    }

    public async Task<List<Tournament>> GetTournamentsInfoAsync(CancellationToken cancellationToken)
    {
        var rawData = await _apiDataService.GetMarketsDataAsync(cancellationToken);

        var tournaments = _prematchParser.ParseTournamentsPrematchData(rawData);
        var matches = _prematchParser.ParseMatchesPrematchData(rawData);
        _prematchParser.LinkMatchesToTournaments(tournaments, matches);

        return tournaments;
    }

    public async Task<List<TournamentResult>> GetTournamentsResultsInfoAsync(CancellationToken cancellationToken)
    {
        var rawData = await _apiDataService.GetResultsDataAsync(cancellationToken);

        var tournaments = _resultsParser.ParseTournamentResultsData(rawData);
        var matches = _resultsParser.ParseMatchResultsData(rawData);
        _resultsParser.LinkMatchResultsToTournamentResults(tournaments, matches);

        return tournaments;
    }
}