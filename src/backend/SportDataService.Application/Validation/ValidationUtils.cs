namespace SportDataService.Application.Validation;

internal static class ValidationUtils
{
    internal static string EmptyParamMessage(string paramName)
    {
        return $"{paramName} can't be empty.";
    }

    internal static string TooShortParamMessage(string paramName, int length)
    {
        return $"{paramName} should be at least {length} symbols long.";
    }

    internal static string TooLongParamMessage(string paramName, int length)
    {
        return $"{paramName} can't be longer than {length} symbols.";
    }

    internal static string TooSmallValueParamMessage(string paramName, decimal value)
    {
        return $"{paramName} should be greater than {value} or equal to {value}.";
    }

    internal static string TooLargeValueParamMessage(string paramName, decimal value)
    {
        return $"{paramName} should be less than {value} or equal to {value}.";
    }
}