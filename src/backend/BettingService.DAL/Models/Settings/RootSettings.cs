namespace BettingService.DAL.Models.Settings;

public class RootSettings
{
    public JwtSettings JwtSettings { get; set; }
    public DatabaseSettings DatabaseSettings { get; set; }
    public HangfireSettings HangfireSettings { get; set; }
}