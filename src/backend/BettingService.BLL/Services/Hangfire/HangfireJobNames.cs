namespace BettingService.BLL.Services.Hangfire;

public static class HangfireJobNames
{
    public const string UpdatePendingBets = "UpdatePendingBets";
    public const string UpdateActiveBets = "UpdateActiveBets";
    public const string ProcessPayouts = "ProcessPayouts";
}