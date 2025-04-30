using Grpc.Core;

namespace SportDataService.GrpcService.Exceptions;

public class SubscriptionFailedException : GrpcExceptionBase
{
    public SubscriptionFailedException(string message) 
        : base(StatusCode.DataLoss, 
            $"Odds subscription failed: {message}") {}
}