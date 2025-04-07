using AutoMapper;
using MongoDB.Bson;
using SportDataService.Application.Contracts.UseCases.Tournament;
using SportDataService.Application.DTO.Tournament;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.Tournament;

public sealed class GetTournamentByIdUseCase(
    ITournamentRepository tournamentRepository,
    IMapper mapper)
    : IGetTournamentByIdUseCase
{
    public async Task<TournamentGetDto> ExecuteAsync(string id, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            throw new ArgumentException("Invalid ID format.");
        }

        cancellationToken.ThrowIfCancellationRequested();

        var tournamentToGet = await tournamentRepository.GetByIdAsync(id, cancellationToken);
        if (tournamentToGet == null)
        {
            throw new TournamentNotFoundByIdException(id);
        }

        return mapper.Map<TournamentGetDto>(tournamentToGet);
    }
}