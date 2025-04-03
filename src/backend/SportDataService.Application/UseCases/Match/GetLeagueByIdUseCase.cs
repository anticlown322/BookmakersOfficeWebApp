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
    public async Task<MatchGetDto> ExecuteAsync(string matchId, CancellationToken cancellationToken)
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

        return mapper.Map<MatchGetDto>(match);
    }
}