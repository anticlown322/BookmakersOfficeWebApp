using Grpc.Core;

namespace SportDataService.GrpcService.Exceptions;

public class OddsChangedException : GrpcExceptionBase
{
    public OddsChangedException(double currentOdds) 
        : base(StatusCode.Aborted, $"Odds changed. Current value: {currentOdds}") {}
}