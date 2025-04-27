using SportDataService.Application.DTO.Results;

namespace SportDataService.Application.Contracts.UseCases.MatchResult;

public interface IGetMatchResultByResultIdUseCase
{
    Task<MatchResultGetDto> ExecuteAsync(string matchResultId, CancellationToken cancellationToken);
}