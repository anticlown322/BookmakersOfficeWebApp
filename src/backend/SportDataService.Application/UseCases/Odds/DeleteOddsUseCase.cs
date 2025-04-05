using MongoDB.Bson;
using SportDataService.Application.Contracts.UseCases.Odds;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.Odds;

public sealed class DeleteOddsUseCase(
    IOddsRepository oddsRepository)
    : IDeleteOddsUseCase
{
    public async Task ExecuteAsync(string oddsId, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(oddsId, out _))
        {
            throw new ArgumentException("Invalid Odds ID format.");
        }

        var oddsToDelete = await oddsRepository.GetByIdAsync(oddsId, cancellationToken);
        if (oddsToDelete == null)
        {
            throw new OddsNotFoundByIdException(oddsId);
        }

        await oddsRepository.DeleteAsync(oddsId, cancellationToken);
    }
}