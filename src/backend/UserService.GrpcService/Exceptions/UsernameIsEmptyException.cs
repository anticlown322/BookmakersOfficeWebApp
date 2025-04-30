using Grpc.Core;

namespace UserService.GrpcService.Exceptions;

public class UsernameIsEmptyException : GrpcExceptionBase
{
    public UsernameIsEmptyException()
        : base(StatusCode.InvalidArgument, "Username cannot be empty")
    {
    }
}