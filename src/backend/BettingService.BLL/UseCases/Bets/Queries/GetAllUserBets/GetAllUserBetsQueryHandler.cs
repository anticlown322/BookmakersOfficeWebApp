using AutoMapper;
using BettingService.BLL.DTO.Bet;
using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.RequestFeatures;
using MediatR;

namespace BettingService.BLL.UseCases.Bets.Queries.GetAllUserBets;

public sealed class GetAllUserBetsQueryHandler(
    IBetRepository betRepository,
    IMapper mapper)
    : IRequestHandler<GetAllUserBetsQuery, PagedResponse<IEnumerable<GetBetDto>>>
{
    public async Task<PagedResponse<IEnumerable<GetBetDto>>> Handle(
        GetAllUserBetsQuery request,
        CancellationToken cancellationToken)
    {
        var betsPagedList = await betRepository.GetUserBetsAsync(request.Parameters, request.Username, cancellationToken);

        var betsDto = betsPagedList.Select(mapper.Map<GetBetDto>);

        return new PagedResponse<IEnumerable<GetBetDto>>(
            betsDto,
            betsPagedList.MetaData);
    }
}