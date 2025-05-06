using Grpc.Core;

namespace SportDataService.GrpcService.Exceptions;

public class InvalidOddsFormatException : GrpcExceptionBase
{
    public InvalidOddsFormatException(string odds) 
        : base(StatusCode.InvalidArgument, 
            $"Invalid odds format for {odds}") {}
}