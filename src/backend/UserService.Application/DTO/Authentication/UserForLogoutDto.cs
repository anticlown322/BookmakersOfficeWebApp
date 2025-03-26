namespace UserService.Application.DTO.Authentication;

public record UserForLogoutDto
{
    public string? UserName { get; init; }
}
