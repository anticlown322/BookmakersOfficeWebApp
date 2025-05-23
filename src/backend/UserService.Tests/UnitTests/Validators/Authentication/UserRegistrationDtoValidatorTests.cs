using FluentValidation.TestHelper;
using UserService.Application.DTO.Authentication;
using UserService.Application.Validation.Validators.Authentication;

namespace UserService.Tests.UnitTests.Validators.Authentication;

public class UserRegistrationDtoValidatorTests
{
    private readonly UserRegistrationDtoValidator _validator = new();

    [Fact]
    public void FirstName_ShouldHaveError_WhenEmpty()
    {
        // Arrange
        var model = new UserRegistrationDto 
        { 
            FirstName = "", 
            LastName = "Doe", 
            UserName = "johndoe",
            Password = "Password123",
            Email = "john@example.com",
            PhoneNumber = "+1234567890"
        };
        
        // Act
        var result = _validator.TestValidate(model);
        
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void FirstName_ShouldHaveError_WhenExceedsMaxLength()
    {
        // Arrange
        var model = new UserRegistrationDto 
        { 
            FirstName = new string('a', 101), 
            LastName = "Doe",
            UserName = "johndoe",
            Password = "Password123",
            Email = "john@example.com",
            PhoneNumber = "+1234567890"
        };
        
        // Act
        var result = _validator.TestValidate(model);
        
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void LastName_ShouldHaveError_WhenEmpty()
    {
        // Arrange
        var model = new UserRegistrationDto 
        { 
            FirstName = "John", 
            LastName = "",
            UserName = "johndoe",
            Password = "Password123",
            Email = "john@example.com",
            PhoneNumber = "+1234567890"
        };
        
        // Act
        var result = _validator.TestValidate(model);
        
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void UserName_ShouldHaveError_WhenExceedsMaxLength()
    {
        // Arrange
        var model = new UserRegistrationDto 
        { 
            FirstName = "John",
            LastName = "Doe",
            UserName = new string('a', 101),
            Password = "Password123",
            Email = "john@example.com",
            PhoneNumber = "+1234567890"
        };
        
        // Act
        var result = _validator.TestValidate(model);
        
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserName);
    }

    [Theory]
    [InlineData("short")]
    [InlineData("tooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooolong")]
    public void Password_ShouldHaveError_WhenInvalidLength(string password)
    {
        // Arrange
        var model = new UserRegistrationDto 
        { 
            FirstName = "John",
            LastName = "Doe",
            UserName = "johndoe",
            Password = password,
            Email = "john@example.com",
            PhoneNumber = "+1234567890"
        };
        
        // Act
        var result = _validator.TestValidate(model);
        
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Email_ShouldHaveError_WhenInvalidFormat()
    {
        // Arrange
        var model = new UserRegistrationDto 
        { 
            FirstName = "John",
            LastName = "Doe",
            UserName = "johndoe",
            Password = "Password123",
            Email = "invalid-email",
            PhoneNumber = "+1234567890"
        };
        
        // Act
        var result = _validator.TestValidate(model);
        
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("12345")]
    [InlineData("+1234567890123456")]
    [InlineData("invalidphone")]
    public void PhoneNumber_ShouldHaveError_WhenInvalid(string phone)
    {
        // Arrange
        var model = new UserRegistrationDto 
        { 
            FirstName = "John",
            LastName = "Doe",
            UserName = "johndoe",
            Password = "Password123",
            Email = "john@example.com",
            PhoneNumber = phone
        };
        
        // Act
        var result = _validator.TestValidate(model);
        
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Fact]
    public void ShouldNotHaveErrors_WhenAllFieldsValid()
    {
        // Arrange
        var model = new UserRegistrationDto 
        { 
            FirstName = "John",
            LastName = "Doe",
            UserName = "johndoe",
            Password = "ValidPass123",
            Email = "john.doe@example.com",
            PhoneNumber = "+1234567890"
        };
        
        // Act
        var result = _validator.TestValidate(model);
        
        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}