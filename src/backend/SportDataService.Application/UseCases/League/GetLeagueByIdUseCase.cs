using AutoMapper;
using MongoDB.Bson;
using SportDataService.Application.Contracts.UseCases.League;
using SportDataService.Application.DTO.League;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.League;

public sealed class GetLeagueByIdUseCase(
    ILeagueRepository leagueRepository,
    IMapper mapper)
    : IGetLeagueByIdUseCase
{
    public async Task<LeagueGetDto> ExecuteAsync(string leagueId, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(leagueId, out _))
        {
            throw new ArgumentException("Invalid League ID format.");
        }

        var league = await leagueRepository.GetByIdAsync(leagueId, cancellationToken);
        if (league == null)
        {
            throw new LeagueNotFoundByIdException(leagueId);
        }

        return mapper.Map<LeagueGetDto>(league);
    }
}