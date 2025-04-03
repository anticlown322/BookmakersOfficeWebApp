using AutoMapper;
using SportDataService.Application.Contracts.UseCases.Match;
using SportDataService.Application.DTO.Match;
using SportDataService.Application.DTO.Match;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Domain.RequestFeatures;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Application.UseCases.Match;

public sealed class GetAllMatchesUseCase(
    IMatchRepository matchRepository,
    IMapper mapper)
    : IGetAllMatchesUseCase
{
    public async Task<(IEnumerable<MatchGetDto> matches, MetaData metaData)> ExecuteAsync(MatchParameters matchParameters, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var matchesWithMetaData = await matchRepository.FindAllMatchesAsync(matchParameters, cancellationToken);

        var matchGetDtos = mapper.Map<IEnumerable<MatchGetDto>>(matchesWithMetaData);

        return (
            matches: matchGetDtos,
            metaData: matchesWithMetaData.MetaData);
    }
}