using AutoMapper;
using BettingService.BLL.DTO.Bet;
using BettingService.BLL.Exceptions.Specific;
using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.RequestFeatures;
using BettingService.Protos;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BettingService.BLL.UseCases.Bets.Queries.GetAllUserBets;

public sealed class GetAllUserBetsQueryHandler(
    IBetRepository betRepository,
    IMapper mapper,
    UserGrpcService.UserGrpcServiceClient userClient,
    ILogger<GetAllUserBetsQueryHandler> logger)
    : IRequestHandler<GetAllUserBetsQuery, PagedResponse<IEnumerable<GetBetDto>>>
{
    public async Task<PagedResponse<IEnumerable<GetBetDto>>> Handle(
        GetAllUserBetsQuery request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"Getting bets for user {request.Username}");

        var balanceResponse = await userClient.GetUserBalanceAsync(
            new GetUserBalanceRequest { Username = request.Username },
            cancellationToken: cancellationToken);

        if (!balanceResponse.UserExists)
        {
            logger.LogWarning($"User {request.Username} does not exist");

            throw new UserNotFoundByNameException(request.Username);
        }

        var betsPagedList = await betRepository.GetUserBetsAsync(
            request.Parameters,
            request.Username,
            cancellationToken);

        var betsDto = betsPagedList.Select(mapper.Map<GetBetDto>);

        logger.LogInformation($"Successfully retrieved all bets for user {request.Username}");

        return new PagedResponse<IEnumerable<GetBetDto>>(
            betsDto,
            betsPagedList.MetaData);
    }
}