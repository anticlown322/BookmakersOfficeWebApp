using System.Text.RegularExpressions;
using SportDataService.Domain.Models.Tournaments;

namespace SportDataService.Application.Contracts.Services;

public interface IDataCollectionService
{
    Task<List<Tournament>> GetTournamentsInfoAsync(CancellationToken cancellationToken);
}