namespace SportDataService.Application.Validation.Exceptions.Base;

public abstract class BadRequestException : Exception
{
    protected BadRequestException(string message)
        : base(message)
    {
    }
}