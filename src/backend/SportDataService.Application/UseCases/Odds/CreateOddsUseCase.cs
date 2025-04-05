using AutoMapper;
using MongoDB.Bson;
using SportDataService.Application.Contracts.UseCases.Odds;
using SportDataService.Application.DTO.Odds;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.Odds;

public sealed class CreateOddsUseCase(
    IOddsRepository oddsRepository,
    IMapper mapper) : ICreateOddsUseCase
{
    public async Task<OddsGetDto> ExecuteAsync(OddsCreateDto oddsCreateDto, CancellationToken cancellationToken)
    {
        var oddsToCreate = mapper.Map<Domain.Models.Odds>(oddsCreateDto);
        oddsToCreate.Id = ObjectId.GenerateNewId().ToString();

        cancellationToken.ThrowIfCancellationRequested();

        await oddsRepository.CreateAsync(oddsToCreate, cancellationToken);

        var oddsGetDto = mapper.Map<OddsGetDto>(oddsToCreate);
        return oddsGetDto;
    }
}