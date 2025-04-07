namespace SportDataService.Domain.Models.Settings;

public class DatabaseSettings
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public int TimeoutSeconds { get; set; }
}