using AutoMapper;
using SportDataService.Application.Contracts.UseCases.Tournament;
using SportDataService.Application.DTO.Tournament;
using SportDataService.Application.DTO.Tournament;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Domain.RequestFeatures;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Application.UseCases.Tournament;

public sealed class GetAllTournamentsUseCase(
    ITournamentRepository tournamentRepository,
    IMapper mapper)
    : IGetAllTournamentsUseCase
{
    public async Task<(IEnumerable<TournamentGetDto> tournaments, MetaData metaData)> ExecuteAsync(TournamentParameters tournamentParameters, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var tournamentsWithMetaData = await tournamentRepository.FindAllTournamentsAsync(tournamentParameters, cancellationToken);

        var tournamentGetDtos = mapper.Map<IEnumerable<TournamentGetDto>>(tournamentsWithMetaData);

        return (
            tournaments: tournamentGetDtos,
            metaData: tournamentsWithMetaData.MetaData);
    }
}