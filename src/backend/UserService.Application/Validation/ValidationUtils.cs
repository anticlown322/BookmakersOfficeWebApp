namespace UserService.Application.Validation;

public static class ValidationUtils
{
    public const int MaxFirstNameLength = 100;
    public const int MaxLastNameLength = 100;
    public const int MaxUsernameLength = 100;
    public const int MinPasswordLength = 8;
    public const int MaxPasswordLength = 100;
    public const int MinPhoneNumberLength = 10;
    public const int MaxPhoneNumberLength = 15;
    public const decimal MinBalanceAmount = 1;
    public const decimal MaxBalanceAmount = 10_000_000;
    public const int MaxCommentLength = 255;

    public static string EmptyParamMessage(string paramName)
    {
        return $"{paramName} can't be empty.";
    }

    public static string TooShortParamMessage(string paramName, int length)
    {
        return $"{paramName} should be at least {length} symbols long.";
    }

    public static string TooLongParamMessage(string paramName, int length)
    {
        return $"{paramName} can't be longer than {length} symbols.";
    }

    public static string TooSmallValueParamMessage(string paramName, decimal value)
    {
        return $"{paramName} should be greater than {value} or equal to {value}.";
    }

    public static string TooLargeValueParamMessage(string paramName, decimal value)
    {
        return $"{paramName} should be less than {value} or equal to {value}.";
    }
}