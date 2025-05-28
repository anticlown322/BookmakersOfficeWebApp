using System.Net;
using BettingService.BLL.Contracts.Services;
using BettingService.BLL.Exceptions.Base;
using BettingService.BLL.Models;
using Grpc.Core;
using Microsoft.AspNetCore.Diagnostics;

namespace BettingService.API.Middlewares;

public static class ExceptionMiddleware
{
    public static void ConfigureExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                if (contextFeature != null)
                {
                    if (contextFeature.Error is RpcException rpcException)
                    {
                        var (statusCode, message) = MapRpcException(rpcException);
                        context.Response.StatusCode = statusCode;

                        await context.Response.WriteAsJsonAsync(
                            new ErrorDetails
                            {
                                StatusCode = statusCode,
                                Message = message,
                            });

                        return;
                    }

                    context.Response.StatusCode = contextFeature.Error switch
                    {
                        ValidationAppException => StatusCodes.Status422UnprocessableEntity,
                        NotFoundException => StatusCodes.Status404NotFound,
                        BadRequestException => StatusCodes.Status400BadRequest,
                        UnauthorizedException => StatusCodes.Status401Unauthorized,
                        _ => StatusCodes.Status500InternalServerError
                    };

                    if (contextFeature.Error is ValidationAppException validationException)
                    {
                        await context.Response.WriteAsJsonAsync(new { validationException.ValidationErrors });

                        return;
                    }

                    await context.Response.WriteAsync(
                        new ErrorDetails
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = contextFeature.Error.Message,
                        }.ToString());
                }
            });
        });
    }

    private static (int statusCode, string message) MapRpcException(RpcException ex)
    {
        return ex.StatusCode switch
        {
            StatusCode.InvalidArgument => (StatusCodes.Status400BadRequest, ex.Status.Detail),
            StatusCode.NotFound => (StatusCodes.Status404NotFound, ex.Status.Detail),
            StatusCode.PermissionDenied => (StatusCodes.Status403Forbidden, ex.Status.Detail),
            StatusCode.Unauthenticated => (StatusCodes.Status401Unauthorized, ex.Status.Detail),
            StatusCode.FailedPrecondition => (StatusCodes.Status412PreconditionFailed, ex.Status.Detail),
            StatusCode.AlreadyExists => (StatusCodes.Status409Conflict, ex.Status.Detail),
            StatusCode.Unimplemented => (StatusCodes.Status501NotImplemented, ex.Status.Detail),
            StatusCode.Unavailable => (StatusCodes.Status503ServiceUnavailable, ex.Status.Detail),
            _ => (StatusCodes.Status500InternalServerError, ex.Status.Detail ?? "Internal server error")
        };
    }
}