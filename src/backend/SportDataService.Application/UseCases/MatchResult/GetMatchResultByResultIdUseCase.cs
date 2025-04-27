using AutoMapper;
using SportDataService.Application.Contracts.UseCases.MatchResult;
using SportDataService.Application.DTO.Results;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.MatchResult;

public class GetMatchResultByResultIdUseCase(
    IMatchResultRepository matchResultRepository,
    IMapper mapper)
    : IGetMatchResultByResultIdUseCase
{
    public async Task<MatchResultGetDto> ExecuteAsync(string matchResultId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var matchResult = await matchResultRepository.GetMatchResultByMatchResultIdAsync(matchResultId, cancellationToken);
        if (matchResult == null)
        {
            throw new MatchResultNotFoundByMatchResultIdException(matchResultId);
        }

        return mapper.Map<MatchResultGetDto>(matchResult);
    }
}