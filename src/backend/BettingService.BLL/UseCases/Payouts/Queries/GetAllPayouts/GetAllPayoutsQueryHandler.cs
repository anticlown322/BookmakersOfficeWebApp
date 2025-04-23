using AutoMapper;
using BettingService.BLL.DTO.Payout;
using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.RequestFeatures;
using MediatR;

namespace BettingService.BLL.UseCases.Payouts.Queries.GetAllPayouts;

public sealed class GetAllPayoutsQueryHandler(
    IPayoutRepository payoutRepository,
    IMapper mapper)
    : IRequestHandler<GetAllPayoutsQuery, PagedResponse<IEnumerable<GetPayoutDto>>>
{
    public async Task<PagedResponse<IEnumerable<GetPayoutDto>>> Handle(
        GetAllPayoutsQuery request,
        CancellationToken cancellationToken)
    {
        var payoutsPagedList = await payoutRepository.GetAllPayoutsAsync(request.Parameters, cancellationToken);

        var payoutsDto = payoutsPagedList.Select(mapper.Map<GetPayoutDto>);

        return new PagedResponse<IEnumerable<GetPayoutDto>>(
            payoutsDto,
            payoutsPagedList.MetaData);
    }
}