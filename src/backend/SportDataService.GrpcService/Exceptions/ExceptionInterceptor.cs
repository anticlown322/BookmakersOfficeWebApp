using Grpc.Core;
using Grpc.Core.Interceptors;
using SportDataService.Application.Validation.Exceptions.Specific;

namespace SportDataService.GrpcService.Exceptions;

public class ExceptionInterceptor : Interceptor
{
    private readonly ILogger<ExceptionInterceptor> _logger;

    public ExceptionInterceptor(ILogger<ExceptionInterceptor> logger)
    {
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogWarning(ex, "Validation error in {Method}", context.Method);
            throw new RpcException(new Status(StatusCode.InvalidArgument, $"Missing required parameter: {ex.ParamName}"));
        }
        catch (InvalidIdFormatException ex)
        {
            _logger.LogWarning("Invalid ID format: {Id}", ex.Message);
            throw new RpcException(new Status(StatusCode.InvalidArgument, $"Invalid ID format: {ex.Message}"));
        }
        catch (MatchNotFoundException ex)
        {
            _logger.LogWarning("Match not found: {MatchId}", ex.Message);
            throw new RpcException(new Status(StatusCode.NotFound, $"Match not found: {ex.Message}"));
        }
        catch (InvalidOddsFormatException ex)
        {
            _logger.LogWarning("Invalid odds format: {Odds}", ex.Message);
            throw new RpcException(new Status(StatusCode.InvalidArgument, $"Invalid odds format: {ex.Message}"));
        }
        catch (OddsChangedException ex)
        {
            _logger.LogWarning("Odds changed. Current: {CurrentOdds}", ex.Message);
            throw new RpcException(new Status(StatusCode.FailedPrecondition, $"Odds changed. Current value: {ex.Message}"));
        }
        catch (MatchAlreadyStartedException ex)
        {
            _logger.LogWarning("Match already started at {StartTime}", ex.Message);
            throw new RpcException(new Status(StatusCode.FailedPrecondition, $"Match already started at {ex.Message}"));
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Method}", context.Method);
            throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
        }
    }
}