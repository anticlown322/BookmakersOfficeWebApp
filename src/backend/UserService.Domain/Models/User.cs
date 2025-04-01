using Microsoft.AspNetCore.Identity;

namespace UserService.Domain.Models;

public class User : IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }

    public virtual UserProfile Profile { get; set; } = new UserProfile();
    public virtual UserBalance Balance { get; set; } = new UserBalance();
}