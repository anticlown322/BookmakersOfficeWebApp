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
    public async Task<TeamGetDto> ExecuteAsync(string id, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            throw new ArgumentException("Invalid ID format.");
        }

        cancellationToken.ThrowIfCancellationRequested();

        var team = await teamRepository.GetByIdAsync(id, cancellationToken);
        if (team == null)
        {
            throw new TeamNotFoundByIdException(id);
        }

        return mapper.Map<TeamGetDto>(team);
    }
}