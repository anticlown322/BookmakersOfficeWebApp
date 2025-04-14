using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SportDataService.Application.Validation;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class ValidationFilterAttribute<T> : Attribute, IAsyncActionFilter
    where T : class
{
    public ValidationFilterAttribute()
    { }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var action = context.RouteData.Values["action"];
        var controller = context.RouteData.Values["controller"];

        if (context.HttpContext.RequestServices.GetService(typeof(IValidator<T>)) is not IValidator<T> validator)
        {
            context.Result = new BadRequestObjectResult(
                $"Validator for {typeof(T).Name} can not be found. Controller: {controller}, action: {action}");

            return;
        }

        var dtoValue = context.ActionArguments.Values.FirstOrDefault(x => x is T);
        if (dtoValue is not T model)
        {
            context.Result = new BadRequestObjectResult(
                $"Model type {dtoValue.GetType().Name} does not match with type {typeof(T).Name}. " +
                $"Controller: {controller}, action: {action}");

            return;
        }

        var result = await validator.ValidateAsync(model);

        if (!result.IsValid)
        {
            var errors = result.Errors
                .GroupBy(vf => vf.PropertyName)
                .ToDictionary(g => g.Key, g => g.First().ErrorMessage);

            context.Result = new BadRequestObjectResult(errors);
        }
        else
        {
            await next();
        }
    }
}