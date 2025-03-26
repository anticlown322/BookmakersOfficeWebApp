namespace UserService.Application.DTO.Authentication;

public record UserForLoginDto
{
    public string? UserName { get; init; }
    public string? Password { get; init; }
}
