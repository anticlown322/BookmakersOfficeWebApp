using Grpc.Core;

namespace SportDataService.GrpcService.Exceptions;

public class InvalidLineTypeException : GrpcExceptionBase
{
    public InvalidLineTypeException(string lineType) 
        : base(StatusCode.InvalidArgument, 
            $"Invalid line type: {lineType}. Valid values: Main/Kills/Maps/Special") {}
}