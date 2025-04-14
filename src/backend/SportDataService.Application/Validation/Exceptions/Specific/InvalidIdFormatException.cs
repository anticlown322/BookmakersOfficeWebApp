using SportDataService.Application.Validation.Exceptions.Base;

namespace SportDataService.Application.Validation.Exceptions.Specific;

public class InvalidIdFormatException : BadRequestException
{
    public InvalidIdFormatException(string id)
        : base($"Id {id} has invalid format.")
    {
    }
}