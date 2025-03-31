namespace UserService.Application.DTO.Authentication;

public record UserLogoutDto
{
    public string? UserName { get; init; }
}
