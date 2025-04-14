using System.Security.Claims;
using Hangfire.Dashboard;
using SportDataService.Infrastructure.Utility;

namespace SportDataService.Infrastructure.Services.Hangfire;

public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        return httpContext.User.Identity?.IsAuthenticated == true
               && httpContext.User.IsInRole(UserRoles.Administrator);
    }
}