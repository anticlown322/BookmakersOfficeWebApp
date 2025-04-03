using AutoMapper;
using MongoDB.Bson;
using SportDataService.Application.Contracts.UseCases.League;
using SportDataService.Application.DTO.League;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.League;

public sealed class CreateLeagueUseCase(
    ILeagueRepository leagueRepository,
    IMapper mapper) : ICreateLeagueUseCase
{
    public async Task<LeagueGetDto> ExecuteAsync(LeagueCreateDto leagueCreateDto, CancellationToken cancellationToken)
    {
        var league = mapper.Map<Domain.Models.League>(leagueCreateDto);
        league.Id = ObjectId.GenerateNewId().ToString();

        cancellationToken.ThrowIfCancellationRequested();

        await leagueRepository.CreateAsync(league, cancellationToken);

        var leagueGetDto = mapper.Map<LeagueGetDto>(league);
        return leagueGetDto;
    }
}