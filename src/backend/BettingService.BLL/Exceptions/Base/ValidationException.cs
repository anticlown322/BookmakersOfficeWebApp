namespace BettingService.BLL.Exceptions.Base;

public class ValidationAppException(IReadOnlyDictionary<string, string[]> validationErrors)
    : Exception("Validation Error!")
{
    public IReadOnlyDictionary<string, string[]> ValidationErrors { get; } = validationErrors;
}