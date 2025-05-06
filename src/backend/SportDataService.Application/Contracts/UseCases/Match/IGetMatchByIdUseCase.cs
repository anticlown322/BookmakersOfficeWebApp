using SportDataService.Application.DTO.Prematch;

namespace SportDataService.Application.Contracts.UseCases.Match;

public interface IGetMatchByIdUseCase
{
    Task<MatchGetDto> ExecuteAsync(string id, CancellationToken cancellationToken);
}