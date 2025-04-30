using Grpc.Core;

namespace UserService.GrpcService.Exceptions;

public class UserServiceException : GrpcExceptionBase
{
    public UserServiceException(string message)
        : base(StatusCode.Internal, message)
    {
    }
}