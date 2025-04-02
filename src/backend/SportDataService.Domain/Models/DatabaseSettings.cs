namespace SportDataService.Domain.Models;

public class DatabaseSettings
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public int TimeoutSeconds { get; set; }
}