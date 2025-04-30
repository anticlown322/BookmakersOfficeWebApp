using Grpc.Core;

namespace SportDataService.GrpcService.Exceptions;

public class SportDataServiceException : GrpcExceptionBase
{
    public SportDataServiceException(string message) 
        : base(StatusCode.Internal, message) {}
}