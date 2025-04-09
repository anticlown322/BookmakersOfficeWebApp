namespace SportDataService.Domain.Models.Settings;

public class HangfireSettings
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public bool EnableDashboard { get; set; }
    public string DashboardPath { get; set; }
    public Dictionary<string, string>? RecurringJobs { get; set; }
}