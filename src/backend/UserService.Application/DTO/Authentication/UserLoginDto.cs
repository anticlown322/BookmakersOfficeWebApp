namespace UserService.Application.DTO.Authentication;

public record UserLoginDto
{
    public string? UserName { get; init; }
    public string? Password { get; init; }
}
