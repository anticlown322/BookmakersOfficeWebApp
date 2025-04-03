using AutoMapper;
using MongoDB.Bson;
using SportDataService.Application.Contracts.UseCases.Team;
using SportDataService.Application.DTO.Team;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.Team;

public sealed class GetTeamByIdUseCase(
    ITeamRepository teamRepository,
    IMapper mapper)
    : IGetTeamByIdUseCase
{
    public async Task<TeamGetDto> ExecuteAsync(string teamId, CancellationToken cancellationToken)
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

        return mapper.Map<TeamGetDto>(team);
    }
}