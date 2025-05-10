namespace SportDataService.Domain.Models.Settings;

public class CacheSettings
{
    public string InstanceName { get; set; }
    public int DefaultCacheMinutes { get; set; }
    public int ConnectTimeoutMs { get; set; }
    public int SyncTimeoutMs { get; set; }
    public bool AbortOnConnectFail { get; set; }
}