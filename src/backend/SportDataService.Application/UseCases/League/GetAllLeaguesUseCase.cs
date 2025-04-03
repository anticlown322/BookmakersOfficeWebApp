using AutoMapper;
using SportDataService.Application.Contracts.UseCases.League;
using SportDataService.Application.DTO.League;
using SportDataService.Application.DTO.League;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Domain.RequestFeatures;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Application.UseCases.League;

public sealed class GetAllLeaguesUseCase(
    ILeagueRepository leagueRepository,
    IMapper mapper)
    : IGetAllLeaguesUseCase
{
    public async Task<(IEnumerable<LeagueGetDto> leagues, MetaData metaData)> ExecuteAsync(LeagueParameters leagueParameters, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var leaguesWithMetaData = await leagueRepository.FindAllLeaguesAsync(leagueParameters, cancellationToken);

        var leagueGetDtos = mapper.Map<IEnumerable<LeagueGetDto>>(leaguesWithMetaData);

        return (
            leagues: leagueGetDtos,
            metaData: leaguesWithMetaData.MetaData);
    }
}