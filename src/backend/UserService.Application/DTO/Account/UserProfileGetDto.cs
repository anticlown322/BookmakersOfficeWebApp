namespace UserService.Application.DTO.Account;

public class UserProfileGetDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public List<string> Roles { get; set; }
}