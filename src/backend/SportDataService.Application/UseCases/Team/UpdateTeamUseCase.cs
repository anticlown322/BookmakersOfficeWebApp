using AutoMapper;
using MongoDB.Bson;
using SportDataService.Application.Contracts.UseCases.Team;
using SportDataService.Application.DTO.Team;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.Team;

public sealed class UpdateTeamUseCase(
    ITeamRepository teamRepository,
    IMapper mapper)
    : IUpdateTeamUseCase
{
    public async Task ExecuteAsync(string teamId, TeamUpdateDto teamUpdateDto, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(teamId, out _))
        {
            throw new ArgumentException("Invalid Team ID format.");
        }

        var team = await teamRepository.GetByIdAsync(teamId, cancellationToken);
        if (team == null)
        {
            throw new TeamNotFoundByIdException(teamId);
        }

        if (teamUpdateDto.PlayerIdsToAdd != null)
        {
            team.PlayerIds.AddRange(teamUpdateDto.PlayerIdsToAdd);
        }

        if (teamUpdateDto.PlayerIdsToRemove != null)
        {
            team.PlayerIds = team.PlayerIds.Except(teamUpdateDto.PlayerIdsToRemove).ToList();
        }

        mapper.Map(teamUpdateDto, team);
        await teamRepository.UpdateAsync(team, cancellationToken);
    }
}