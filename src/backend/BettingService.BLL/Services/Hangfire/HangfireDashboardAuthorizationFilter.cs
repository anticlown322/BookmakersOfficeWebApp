using BettingService.BLL.Utility;
using Hangfire.Dashboard;

namespace BettingService.BLL.Services.Hangfire;

public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        return httpContext.User.Identity?.IsAuthenticated == true
               && httpContext.User.IsInRole(UserRoles.Administrator);
    }
}