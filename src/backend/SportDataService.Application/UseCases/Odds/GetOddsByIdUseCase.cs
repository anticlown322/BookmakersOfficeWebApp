using AutoMapper;
using MongoDB.Bson;
using SportDataService.Application.Contracts.UseCases.Odds;
using SportDataService.Application.DTO.Odds;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.Odds;

public sealed class GetOddsByIdUseCase(
    IOddsRepository oddsRepository,
    IMapper mapper)
    : IGetOddsByIdUseCase
{
    public async Task<OddsGetDto> ExecuteAsync(string oddsId, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(oddsId, out _))
        {
            throw new ArgumentException("Invalid Odds ID format.");
        }

        var oddsToGet = await oddsRepository.GetByIdAsync(oddsId, cancellationToken);
        if (oddsToGet == null)
        {
            throw new OddsNotFoundByIdException(oddsId);
        }

        return mapper.Map<OddsGetDto>(oddsToGet);
    }
}