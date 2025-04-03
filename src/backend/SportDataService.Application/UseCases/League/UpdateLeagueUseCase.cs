using AutoMapper;
using MongoDB.Bson;
using SportDataService.Application.Contracts.UseCases.League;
using SportDataService.Application.DTO.League;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.League;

public sealed class UpdateLeagueUseCase(
    ILeagueRepository leagueRepository,
    IMapper mapper)
    : IUpdateLeagueUseCase
{
    public async Task ExecuteAsync(string leagueId, LeagueUpdateDto leagueUpdateDto, CancellationToken cancellationToken)
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

        mapper.Map(leagueUpdateDto, league);
        await leagueRepository.UpdateAsync(league, cancellationToken);
    }
}