using SportDataService.Application.DTO.Results;

namespace SportDataService.Application.Contracts.UseCases.MatchResult;

public interface IGetMatchResultByIdUseCase
{
    Task<MatchResultGetDto> ExecuteAsync(string id, CancellationToken cancellationToken);
}