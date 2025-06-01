using Grpc.Core;
using UserService.Domain.RepositoryContracts;
using UserService.GrpcService.Exceptions;

namespace UserService.GrpcService.Services;

public class UserGrpcServiceImplementation(IUsersRepository repository)
    : UserGrpcService.UserGrpcServiceBase
{
    public override async Task<GetUserBalanceResponse> GetUserBalance(
        GetUserBalanceRequest request,
        ServerCallContext context)
    {
        try
        {
            var user = await repository.GetUserByNameAsync(request.Username, CancellationToken.None);

            if (user is null)
            {
                return new GetUserBalanceResponse
                {
                    UserExists = false,
                    Balance = 0,
                };
            }

            return new GetUserBalanceResponse
            {
                UserExists = true,
                Balance = (double)user.Balance.CurrentAmount,
            };
        }
        catch (GrpcExceptionBase)
        {
            throw;
        }
        catch (Exception ex) when (ex is not RpcException)
        {
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }

    public override async Task<UpdateUserBalanceResponse> UpdateUserBalance(
        UpdateUserBalanceRequest request,
        ServerCallContext context)
    {
        try
        {
            decimal amount;
            try
            {
                amount = (decimal)request.Amount;
                if (amount == 0)
                {
                    throw new AmountIsIncorrectException("Amount cannot be zero");
                }
            }
            catch (OverflowException)
            {
                throw new AmountIsIncorrectException("Amount is too large");
            }
            catch (Exception)
            {
                throw new AmountIsIncorrectException();
            }

            var user = await repository.GetUserByNameAsync(request.Username);
            if (user == null)
            {
                throw new UserNotFoundException(request.Username);
            }

            if (user.Balance.CurrentAmount + amount < 0)
            {
                throw new RpcException(new Status(StatusCode.FailedPrecondition, "Insufficient funds"));
            }

            user.Balance.CurrentAmount += amount;
            await repository.UpdateUserAsync(user);

            return new UpdateUserBalanceResponse
            {
                Success = true,
                NewBalance = (double)user.Balance.CurrentAmount,
            };
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }
}