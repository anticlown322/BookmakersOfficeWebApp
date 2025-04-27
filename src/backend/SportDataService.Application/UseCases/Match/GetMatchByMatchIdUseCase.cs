using AutoMapper;
using SportDataService.Application.Contracts.UseCases.Match;
using SportDataService.Application.DTO.Prematch;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.Match;

public class GetMatchByMatchIdUseCase(
    IMatchRepository matchRepository,
    IMapper mapper)
    : IGetMatchByMatchIdUseCase
{
    public async Task<MatchGetDto> ExecuteAsync(string matchId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var match = await matchRepository.GetMatchByMatchIdAsync(matchId, cancellationToken);
        if (match == null)
        {
            throw new MatchNotFoundByMatchIdException(matchId);
        }

        return mapper.Map<MatchGetDto>(match);
    }
}