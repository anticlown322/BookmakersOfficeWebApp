using AutoMapper;
using BettingService.BLL.DTO.Payout;
using BettingService.BLL.Exceptions.Specific;
using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.RequestFeatures;
using BettingService.Protos;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BettingService.BLL.UseCases.Payouts.Queries.GetAllUserPayouts;

public sealed class GetAllUserPayoutsQueryHandler(
    IPayoutRepository payoutRepository,
    IMapper mapper,
    UserGrpcService.UserGrpcServiceClient userClient,
    ILogger<GetAllUserPayoutsQueryHandler> logger)
    : IRequestHandler<GetAllUserPayoutsQuery, PagedResponse<IEnumerable<GetPayoutDto>>>
{
    public async Task<PagedResponse<IEnumerable<GetPayoutDto>>> Handle(
        GetAllUserPayoutsQuery request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"Getting payouts for user {request.Username}");

        var balanceResponse = await userClient.GetUserBalanceAsync(
            new GetUserBalanceRequest { Username = request.Username },
            cancellationToken: cancellationToken);

        if (!balanceResponse.UserExists)
        {
            logger.LogWarning($"User {request.Username} does not exist");

            throw new UserNotFoundByNameException(request.Username);
        }

        var payoutsPagedList = await payoutRepository.GetUserPayoutsAsync(
            request.Parameters,
            request.Username,
            cancellationToken);

        var payoutsDto = payoutsPagedList.Select(mapper.Map<GetPayoutDto>);

        logger.LogInformation($"Successfully retrieved for user {request.Username}");

        return new PagedResponse<IEnumerable<GetPayoutDto>>(
            payoutsDto,
            payoutsPagedList.MetaData);
    }
}