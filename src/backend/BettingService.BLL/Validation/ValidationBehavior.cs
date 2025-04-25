using BettingService.BLL.Exceptions.Base;
using FluentValidation;
using MediatR;

namespace BettingService.BLL.Validation;

public class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);
        var errorsDictionary = validators
            .Select(v => v.Validate(context))
            .SelectMany(v => v.Errors)
            .Where(e => e is not null)
            .GroupBy(
                e => e.PropertyName.Substring(e.PropertyName.IndexOf('.') + 1),
                e => e.ErrorMessage,
                (propertyName, errorMessages) => new
                {
                    Key = propertyName,
                    Values = errorMessages.Distinct().ToArray(),
                })
            .ToDictionary(x => x.Key, x => x.Values);

        if (errorsDictionary.Count != 0)
        {
            throw new ValidationAppException(errorsDictionary);
        }

        return await next();
    }
}