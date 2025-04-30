using Grpc.Core;
using Grpc.Core.Interceptors;

namespace SportDataService.GrpcService.Exceptions;

public class ExceptionInterceptor : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (GrpcExceptionBase)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new SportDataServiceException($"Internal error: {ex.Message}");
        }
    }
}