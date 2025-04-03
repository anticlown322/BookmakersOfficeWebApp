using AutoMapper;
using MongoDB.Bson;
using SportDataService.Application.Contracts.UseCases.Team;
using SportDataService.Application.DTO.Team;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.Team;

public sealed class CreateTeamUseCase(
    ITeamRepository teamRepository,
    IMapper mapper) : ICreateTeamUseCase
{
    public async Task<TeamGetDto> ExecuteAsync(TeamCreateDto teamCreateDto, CancellationToken cancellationToken)
    {
        var team = mapper.Map<Domain.Models.Team>(teamCreateDto);
        team.Id = ObjectId.GenerateNewId().ToString();

        cancellationToken.ThrowIfCancellationRequested();

        await teamRepository.CreateAsync(team, cancellationToken);

        var teamGetDto = mapper.Map<TeamGetDto>(team);
        return teamGetDto;
    }
}