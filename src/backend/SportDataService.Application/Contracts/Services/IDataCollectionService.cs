using System.Text.RegularExpressions;
using SportDataService.Domain.Models.Prematch;
using SportDataService.Domain.Models.Results;

namespace SportDataService.Application.Contracts.Services;

public interface IDataCollectionService
{
    Task<List<Tournament>> GetTournamentsInfoAsync(CancellationToken cancellationToken);
    Task<List<TournamentResult>> GetTournamentsResultsInfoAsync(CancellationToken cancellationToken);
}