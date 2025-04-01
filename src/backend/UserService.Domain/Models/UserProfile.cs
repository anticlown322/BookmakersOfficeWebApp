namespace UserService.Domain.Models;

public class UserProfile
{
    public int Id { get; set; }
    public string UserId { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public List<string> Roles { get; set; } = new ();

    public virtual User User { get; set; }
}