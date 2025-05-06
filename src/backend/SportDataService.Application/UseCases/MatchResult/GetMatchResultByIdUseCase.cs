using AutoMapper;
using MongoDB.Bson;
using SportDataService.Application.Contracts.UseCases.MatchResult;
using SportDataService.Application.DTO.Results;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.MatchResult;

public sealed class GetMatchResultByIdUseCase(
    IMatchResultRepository matchResultRepository,
    IMapper mapper)
    : IGetMatchResultByIdUseCase
{
    public async Task<MatchResultGetDto> ExecuteAsync(string id, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            throw new InvalidIdFormatException(id);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var matchResult = await matchResultRepository.GetByIdAsync(id, cancellationToken);
        if (matchResult == null)
        {
            throw new MatchResultNotFoundByIdException(id);
        }

        return mapper.Map<MatchResultGetDto>(matchResult);
    }
}