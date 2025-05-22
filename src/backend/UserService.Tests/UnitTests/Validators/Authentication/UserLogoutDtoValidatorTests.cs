using FluentValidation.TestHelper;
using UserService.Application.DTO.Authentication;
using UserService.Application.Validation.Validators.Authentication;

namespace UserService.Tests.UnitTests.Validators.Authentication;

public class UserLogoutDtoValidatorTests
{
    private readonly UserLogoutDtoValidator _validator = new();

    [Fact]
    public void UserName_ShouldHaveError_WhenEmpty()
    {
        var model = new UserLogoutDto { UserName = "" };
        var result = _validator.TestValidate(model);
        
        result.ShouldHaveValidationErrorFor(x => x.UserName)
            .WithErrorMessage("UserName can't be empty.");
    }

    [Fact]
    public void UserName_ShouldHaveError_WhenExceedsMaxLength()
    {
        var longName = new string('a', 101);
        var model = new UserLogoutDto { UserName = longName };
        var result = _validator.TestValidate(model);
        
        result.ShouldHaveValidationErrorFor(x => x.UserName)
            .WithErrorMessage("UserName can't be longer than 100 symbols.");
    }

    [Theory]
    [InlineData("user1")]
    [InlineData("valid-username")]
    [InlineData("user.name")]
    public void ShouldNotHaveErrors_WhenUserNameValid(string username)
    {
        var model = new UserLogoutDto { UserName = username };
        var result = _validator.TestValidate(model);
        
        result.ShouldNotHaveAnyValidationErrors();
    }
}