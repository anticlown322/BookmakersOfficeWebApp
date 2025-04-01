namespace UserService.Presentation.Utility;

public static class AuthorizationPolicies
{
    public const string GamblerOnly = "GamblerOnly";
    public const string ModeratorOnly = "ModeratorOnly";
    public const string BookmakerOnly = "BookmakerOnly";
    public const string AdministratorOnly = "AdministratorOnly";

    public const string AdministratorOrGambler = "AdministratorOrGambler";
    public const string AdministratorOrModerator = "AdministratorOrModerator";
    public const string AdministratorOrModeratorOrGambler = "AdministratorOrModeratorOrGambler";
    public const string AllUsers = "AllUsers";
}