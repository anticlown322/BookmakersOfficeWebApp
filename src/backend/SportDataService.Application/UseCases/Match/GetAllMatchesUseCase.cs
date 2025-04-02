using AutoMapper;
using SportDataService.Application.Contracts.UseCases.Match;
using SportDataService.Application.DTO.Match;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Domain.RequestFeatures;

namespace SportDataService.Application.UseCases.Match;

public sealed class GetAllMatchesUseCase(
    IMatchRepository matchRepository,
    IMapper mapper)
    : IGetAllMatchesUseCase
{
    public async Task<(IEnumerable<MatchGetDto> matches, MetaData metaData)> ExecuteAsync(MatchParameters matchParams, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var matchesWithMetaData = await matchRepository.FindAllMatchesAsync(matchParams, cancellationToken);

        var matchGetDtos = mapper.Map<IEnumerable<MatchGetDto>>(matchesWithMetaData);

        return (
            matches: matchGetDtos,
            metaData: matchesWithMetaData.MetaData);
    }
}