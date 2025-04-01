namespace UserService.Domain.Models;

public class EmailSettings
{
    public string Server { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public bool UseSsl { get; set; }
    public string SenderEmail { get; set; }
    public string SenderName { get; set; }
}