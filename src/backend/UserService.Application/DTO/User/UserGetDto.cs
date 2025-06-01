namespace UserService.Application.DTO.User;

public record UserGetDto
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public List<string> Roles { get; set; }
}
