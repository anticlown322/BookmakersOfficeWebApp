namespace SportDataService.Domain.Models.Settings;

public class CacheSettings
{
    public string InstanceName { get; set; }
    public int DefaultCacheMinutes { get; set; }
}