using Grpc.Core;

namespace SportDataService.GrpcService.Exceptions;

public class MatchAlreadyStartedException : GrpcExceptionBase
{
    public MatchAlreadyStartedException(DateTime startTime) 
        : base(StatusCode.FailedPrecondition, 
            $"Match started at {startTime:yyyy-MM-dd HH:mm}") {}
}