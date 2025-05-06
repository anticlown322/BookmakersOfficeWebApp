using Grpc.Core;

namespace SportDataService.GrpcService.Exceptions;

public class MarketNotFoundException : GrpcExceptionBase
{
    public MarketNotFoundException(string market) 
        : base(StatusCode.FailedPrecondition, $"Market {market} not found") {}
}
