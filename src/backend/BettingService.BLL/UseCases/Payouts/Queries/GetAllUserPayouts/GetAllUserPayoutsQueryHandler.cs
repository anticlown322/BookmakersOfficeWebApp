using AutoMapper;
using BettingService.BLL.DTO.Payout;
using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.RequestFeatures;
using MediatR;

namespace BettingService.BLL.UseCases.Payouts.Queries.GetAllUserPayouts;

public sealed class GetAllUserPayoutsQueryHandler(
    IPayoutRepository payoutRepository,
    IMapper mapper)
    : IRequestHandler<GetAllUserPayoutsQuery, PagedResponse<IEnumerable<GetPayoutDto>>>
{
    public async Task<PagedResponse<IEnumerable<GetPayoutDto>>> Handle(
        GetAllUserPayoutsQuery request,
        CancellationToken cancellationToken)
    {
        // TODO: add validation for user when grpc is implemented

        var payoutsPagedList = await payoutRepository.GetUserPayoutsAsync(
            request.Parameters,
            request.Username,
            cancellationToken);

        var payoutsDto = payoutsPagedList.Select(mapper.Map<GetPayoutDto>);

        return new PagedResponse<IEnumerable<GetPayoutDto>>(
            payoutsDto,
            payoutsPagedList.MetaData);
    }
}