using AutoMapper;
using MongoDB.Bson;
using SportDataService.Application.Contracts.UseCases.Match;
using SportDataService.Application.DTO.Match;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.Match;

public sealed class GetMatchByIdUseCase(
    IMatchRepository matchRepository,
    IMapper mapper)
    : IGetMatchByIdUseCase
{
    public async Task<MatchGetDto> ExecuteAsync(string id, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            throw new InvalidIdFormatException(id);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var match = await matchRepository.GetByIdAsync(id, cancellationToken);
        if (match == null)
        {
            throw new MatchNotFoundByIdException(id);
        }

        return mapper.Map<MatchGetDto>(match);
    }
}