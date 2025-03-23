namespace UserService.Application.Validation;

internal static class ValidationUtils
{
    internal static string EmptyParamMessage(string paramName)
    {
        return $"{paramName} can't be empty.";
    }

    internal static string TooLongParamMessage(string paramName, int length)
    {
        return $"{paramName} can't be longer than {length} symbols.";
    }
}