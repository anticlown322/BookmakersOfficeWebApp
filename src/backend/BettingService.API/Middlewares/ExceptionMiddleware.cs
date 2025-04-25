using System.Net;
using BettingService.BLL.Contracts.Services;
using BettingService.BLL.Exceptions.Base;
using BettingService.BLL.Models;
using Microsoft.AspNetCore.Diagnostics;

namespace BettingService.API.Middlewares;

public static class ExceptionMiddleware
{
    public static void ConfigureExceptionHandler(this WebApplication app, ILoggerService logger)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
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

                    logger.LogError($"Something went wrong: {contextFeature.Error}");

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
}