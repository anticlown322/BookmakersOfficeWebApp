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
        // Arrange
        var model = new UserLogoutDto { UserName = "" };
        
        // Act
        var result = _validator.TestValidate(model);
        
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserName);
    }

    [Fact]
    public void UserName_ShouldHaveError_WhenExceedsMaxLength()
    {
        // Arrange
        var longName = new string('a', 101);
        var model = new UserLogoutDto { UserName = longName };
        
        // Arrange
        var result = _validator.TestValidate(model);
        
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserName);
    }

    [Theory]
    [InlineData("user1")]
    [InlineData("valid-username")]
    [InlineData("user.name")]
    public void ShouldNotHaveErrors_WhenUserNameValid(string username)
    {
        // Arrange
        var model = new UserLogoutDto { UserName = username };
        
        // Act
        var result = _validator.TestValidate(model);
        
        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}