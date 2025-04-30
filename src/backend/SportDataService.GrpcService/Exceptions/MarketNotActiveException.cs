using Grpc.Core;

namespace SportDataService.GrpcService.Exceptions;

public class MarketNotActiveException : GrpcExceptionBase
{
    public MarketNotActiveException(string market) 
        : base(StatusCode.FailedPrecondition, $"Market {market} is not active") {}
}
