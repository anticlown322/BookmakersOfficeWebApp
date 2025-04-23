namespace BettingService.API.Utility;

public static class AuthorizationPolicies
{
    public const string AdministratorOnly = "AdministratorOnly";
    public const string GamblerOnly = "GamblerOnly";
    public const string AdministratorOrGambler = "AdministratorOrGambler";
}