using AutoMapper;
using BettingService.BLL.DTO.Bet;
using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.RequestFeatures;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BettingService.BLL.UseCases.Bets.Queries.GetAllBets;

public sealed class GetAllBetsQueryHandler(
    IBetRepository betRepository,
    IMapper mapper,
    ILogger<GetAllBetsQueryHandler> logger)
    : IRequestHandler<GetAllBetsQuery, PagedResponse<IEnumerable<GetBetDto>>>
{
    public async Task<PagedResponse<IEnumerable<GetBetDto>>> Handle(
        GetAllBetsQuery request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all bets...");

        var betsPagedList = await betRepository
            .GetAllBetsAsync(request.Parameters, cancellationToken);

        var betsDto = betsPagedList.Select(mapper.Map<GetBetDto>);

        logger.LogInformation("Successfully retrieved all bets");

        return new PagedResponse<IEnumerable<GetBetDto>>(
            betsDto,
            betsPagedList.MetaData);
    }
}