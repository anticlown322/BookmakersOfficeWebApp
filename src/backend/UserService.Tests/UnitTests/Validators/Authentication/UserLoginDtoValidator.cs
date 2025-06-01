using FluentValidation.TestHelper;
using UserService.Application.DTO.Authentication;
using UserService.Application.Validation.Validators.Authentication;

namespace UserService.Tests.UnitTests.Validators.Authentication;

public class UserLoginDtoValidatorTests
{
    private readonly UserLoginDtoValidator _validator = new();

    [Fact]
    public void UserName_ShouldHaveError_WhenEmpty()
    {
        // Arrange
        var model = new UserLoginDto { UserName = "", Password = "validPass123" };
        
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
        var model = new UserLoginDto { UserName = longName, Password = "validPass123" };
        
        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserName);
    }

    [Theory]
    [InlineData("user1")]
    [InlineData("valid.username")]
    [InlineData("user@example.com")]
    public void UserName_ShouldNotHaveError_WhenValid(string username)
    {
        // Arrange
        var model = new UserLoginDto{ UserName = username, Password = "validPass123" };
        
        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.UserName);
    }

    [Fact]
    public void Password_ShouldHaveError_WhenEmpty()
    {
        // Arrange
        var model = new UserLoginDto { UserName = "validUser", Password = "" };
        
        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Password_ShouldHaveError_WhenExceedsMaxLength()
    {
        // Arrange
        var longPassword = new string('a', 101);
        var model = new UserLoginDto { UserName = "validUser", Password = longPassword };
        
        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Theory]
    [InlineData("password123")]
    [InlineData("P@ssw0rd")]
    [InlineData("12345678")]
    public void Password_ShouldNotHaveError_WhenValid(string password)
    {
        // Arrange
        var model = new UserLoginDto { UserName = "validUser", Password = password };
        
        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void ShouldNotHaveErrors_WhenAllFieldsValid()
    {
        // Arrange
        var model = new UserLoginDto { UserName = "validUser", Password = "validPass123" };
        
        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}