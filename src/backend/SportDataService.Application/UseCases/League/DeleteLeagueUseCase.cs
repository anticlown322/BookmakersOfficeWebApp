using MongoDB.Bson;
using SportDataService.Application.Contracts.UseCases.League;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.League;

public sealed class DeleteLeagueUseCase(
    ILeagueRepository leagueRepository)
    : IDeleteLeagueUseCase
{
    public async Task ExecuteAsync(string leagueId, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(leagueId, out _))
        {
            throw new ArgumentException("Invalid League ID format.");
        }

        var league = await leagueRepository.GetByIdAsync(leagueId, cancellationToken);
        if (league == null)
        {
            throw new LeagueNotFoundByIdException(leagueId);
        }

        await leagueRepository.DeleteAsync(leagueId, cancellationToken);
    }
}