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
        var model = new UserRegistrationDto 
        { 
            FirstName = "", 
            LastName = "Doe", 
            UserName = "johndoe",
            Password = "Password123",
            Email = "john@example.com",
            PhoneNumber = "+1234567890"
        };
        
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("FirstName can't be empty.");
    }

    [Fact]
    public void FirstName_ShouldHaveError_WhenExceedsMaxLength()
    {
        var model = new UserRegistrationDto 
        { 
            FirstName = new string('a', 101), 
            LastName = "Doe",
            UserName = "johndoe",
            Password = "Password123",
            Email = "john@example.com",
            PhoneNumber = "+1234567890"
        };
        
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("FirstName can't be longer than 100 symbols.");
    }

    [Fact]
    public void LastName_ShouldHaveError_WhenEmpty()
    {
        var model = new UserRegistrationDto 
        { 
            FirstName = "John", 
            LastName = "",
            UserName = "johndoe",
            Password = "Password123",
            Email = "john@example.com",
            PhoneNumber = "+1234567890"
        };
        
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("LastName can't be empty.");
    }

    [Fact]
    public void UserName_ShouldHaveError_WhenExceedsMaxLength()
    {
        var model = new UserRegistrationDto 
        { 
            FirstName = "John",
            LastName = "Doe",
            UserName = new string('a', 101),
            Password = "Password123",
            Email = "john@example.com",
            PhoneNumber = "+1234567890"
        };
        
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.UserName)
            .WithErrorMessage("UserName can't be longer than 100 symbols.");
    }

    [Theory]
    [InlineData("short")]
    [InlineData("tooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooolong")]
    public void Password_ShouldHaveError_WhenInvalidLength(string password)
    {
        var model = new UserRegistrationDto 
        { 
            FirstName = "John",
            LastName = "Doe",
            UserName = "johndoe",
            Password = password,
            Email = "john@example.com",
            PhoneNumber = "+1234567890"
        };
        
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password number must have length 8 to 100");
    }

    [Fact]
    public void Email_ShouldHaveError_WhenInvalidFormat()
    {
        var model = new UserRegistrationDto 
        { 
            FirstName = "John",
            LastName = "Doe",
            UserName = "johndoe",
            Password = "Password123",
            Email = "invalid-email",
            PhoneNumber = "+1234567890"
        };
        
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Invalid email address.");
    }

    [Theory]
    [InlineData("12345")]
    [InlineData("+1234567890123456")]
    [InlineData("invalidphone")]
    public void PhoneNumber_ShouldHaveError_WhenInvalid(string phone)
    {
        var model = new UserRegistrationDto 
        { 
            FirstName = "John",
            LastName = "Doe",
            UserName = "johndoe",
            Password = "Password123",
            Email = "john@example.com",
            PhoneNumber = phone
        };
        
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Fact]
    public void ShouldNotHaveErrors_WhenAllFieldsValid()
    {
        var model = new UserRegistrationDto 
        { 
            FirstName = "John",
            LastName = "Doe",
            UserName = "johndoe",
            Password = "ValidPass123",
            Email = "john.doe@example.com",
            PhoneNumber = "+1234567890"
        };
        
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}