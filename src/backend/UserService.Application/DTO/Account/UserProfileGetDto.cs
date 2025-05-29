namespace UserService.Application.DTO.Account;

public class UserProfileGetDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public List<string> Roles { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public bool isEmailConfirmed { get; set; }
}