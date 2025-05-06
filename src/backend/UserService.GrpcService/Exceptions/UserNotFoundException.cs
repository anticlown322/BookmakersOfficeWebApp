using Grpc.Core;

namespace UserService.GrpcService.Exceptions;

public class UserNotFoundException : GrpcExceptionBase
{
    public UserNotFoundException(string username)
        : base(StatusCode.NotFound, $"User '{username}' not found")
    {
    }
}