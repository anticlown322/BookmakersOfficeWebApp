using MongoDB.Bson;
using SportDataService.Application.Contracts.UseCases.Match;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.Match;

public sealed class DeleteMatchUseCase(
    IMatchRepository matchRepository)
    : IDeleteMatchUseCase
{
    public async Task ExecuteAsync(string matchId, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(matchId, out _))
        {
            throw new ArgumentException("Invalid Match ID format.");
        }

        var match = await matchRepository.GetByIdAsync(matchId, cancellationToken);
        if (match == null)
        {
            throw new MatchNotFoundByIdException(matchId);
        }

        await matchRepository.DeleteAsync(matchId, cancellationToken);
    }
}