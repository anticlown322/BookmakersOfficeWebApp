using AutoMapper;
using MongoDB.Bson;
using SportDataService.Application.Contracts.UseCases.TournamentResult;
using SportDataService.Application.DTO.Prematch;
using SportDataService.Application.DTO.Results;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.TournamentResult;

public sealed class GetTournamentResultByIdUseCase(
    ITournamentResultRepository tournamentResultRepository,
    IMapper mapper)
    : IGetTournamentResultByIdUseCase
{
    public async Task<TournamentResultGetDto> ExecuteAsync(string id, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            throw new InvalidIdFormatException(id);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var tournamentResultToGet = await tournamentResultRepository.GetByIdAsync(id, cancellationToken);
        if (tournamentResultToGet == null)
        {
            throw new TournamentResultNotFoundByIdException(id);
        }

        return mapper.Map<TournamentResultGetDto>(tournamentResultToGet);
    }
}