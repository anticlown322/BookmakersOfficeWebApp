namespace UserService.Application.Validation.Exceptions.Base;

public abstract class UnauthorizedException : Exception
{
    protected UnauthorizedException(string message)
        : base(message)
    {
    }
}