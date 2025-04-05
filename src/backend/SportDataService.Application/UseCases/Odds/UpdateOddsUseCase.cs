using AutoMapper;
using MongoDB.Bson;
using SportDataService.Application.Contracts.UseCases.Odds;
using SportDataService.Application.DTO.Odds;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.Odds;

public sealed class UpdateOddsUseCase(
    IOddsRepository oddsRepository,
    IMapper mapper)
    : IUpdateOddsUseCase
{
    public async Task ExecuteAsync(string oddsId, OddsUpdateDto oddsUpdateDto, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(oddsId, out _))
        {
            throw new ArgumentException("Invalid Odds ID format.");
        }

        var oddsToUpdate = await oddsRepository.GetByIdAsync(oddsId, cancellationToken);
        if (oddsToUpdate == null)
        {
            throw new OddsNotFoundByIdException(oddsId);
        }

        mapper.Map(oddsUpdateDto, oddsToUpdate);
        await oddsRepository.UpdateAsync(oddsToUpdate, cancellationToken);
    }
}