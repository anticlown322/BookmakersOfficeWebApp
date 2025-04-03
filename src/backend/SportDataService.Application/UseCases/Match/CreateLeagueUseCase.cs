using AutoMapper;
using MongoDB.Bson;
using SportDataService.Application.Contracts.UseCases.Match;
using SportDataService.Application.DTO.Match;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.Match;

public sealed class CreateMatchUseCase(
    IMatchRepository matchRepository,
    IMapper mapper) : ICreateMatchUseCase
{
    public async Task<MatchGetDto> ExecuteAsync(MatchCreateDto matchCreateDto, CancellationToken cancellationToken)
    {
        var match = mapper.Map<Domain.Models.Match>(matchCreateDto);
        match.Id = ObjectId.GenerateNewId().ToString();

        cancellationToken.ThrowIfCancellationRequested();

        await matchRepository.CreateAsync(match, cancellationToken);

        var matchGetDto = mapper.Map<MatchGetDto>(match);
        return matchGetDto;
    }
}