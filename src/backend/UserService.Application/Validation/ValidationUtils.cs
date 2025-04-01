namespace UserService.Application.Validation;

internal static class ValidationUtils
{
    internal const int MaxFirstNameLength = 100;
    internal const int MaxLastNameLength = 100;
    internal const int MaxUsernameLength = 100;
    internal const int MinPasswordLength = 8;
    internal const int MaxPasswordLength = 100;
    internal const int MinPhoneNumberLength = 10;
    internal const int MaxPhoneNumberLength = 15;
    internal const decimal MinBalanceAmount = 1;
    internal const decimal MaxBalanceAmount = 10_000_000;
    internal const int MaxCommentLength = 255;

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