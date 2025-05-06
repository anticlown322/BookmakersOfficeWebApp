using Grpc.Core;

namespace SportDataService.GrpcService.Exceptions;

public class MatchNotFoundException : GrpcExceptionBase
{
    public MatchNotFoundException(string matchId) 
        : base(StatusCode.NotFound, $"Match {matchId} not found") {}
}