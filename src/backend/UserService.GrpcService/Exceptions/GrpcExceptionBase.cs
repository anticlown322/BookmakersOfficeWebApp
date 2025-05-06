using Grpc.Core;

namespace UserService.GrpcService.Exceptions;

public abstract class GrpcExceptionBase : RpcException
{
    protected GrpcExceptionBase(StatusCode status, string message)
        : base(new Status(status, message))
    {
    }
}