namespace UserService.Domain.Models;

public class DatabaseSettings
{
    public string ConnectionString { get; set; }
    public int Timeout { get; set; }
}