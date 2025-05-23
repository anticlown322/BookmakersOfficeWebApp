using FluentValidation.TestHelper;
using UserService.Application.DTO.Account;
using UserService.Application.Validation.Validators.Account;

namespace UserService.Tests.UnitTests.Validators.Account;

public class UserProfileUpdateDtoValidatorTests
{
    private readonly UserProfileUpdateDtoValidator _validator = new();

    [Fact]
    public void FirstName_ShouldHaveError_WhenEmpty()
    {
        // Arrange
        var model = new UserProfileUpdateDto { FirstName = "", LastName = "ValidLastName" };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void FirstName_ShouldHaveError_WhenExceedsMaxLength()
    {
        // Arrange
        var longName = new string('a', 101); // 101 chars > 100 max
        var model = new UserProfileUpdateDto { FirstName = longName, LastName = "ValidLastName" };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void FirstName_ShouldNotHaveError_WhenValid()
    {
        // Arrange
        var model = new UserProfileUpdateDto { FirstName = "ValidName", LastName = "ValidLastName" };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void LastName_ShouldHaveError_WhenEmpty()
    {
        // Arrange
        var model = new UserProfileUpdateDto { FirstName = "ValidFirstName", LastName = "" };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void LastName_ShouldHaveError_WhenExceedsMaxLength()
    {
        // Arrange
        var longName = new string('a', 101); // 101 chars > 100 max
        var model = new UserProfileUpdateDto { FirstName = "ValidFirstName", LastName = longName };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void LastName_ShouldNotHaveError_WhenValid()
    {
        // Arrange
        var model = new UserProfileUpdateDto { FirstName = "ValidFirstName", LastName = "ValidLastName" };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void ShouldNotHaveErrors_WhenAllFieldsValid()
    {
        // Arrange
        var model = new UserProfileUpdateDto 
        { 
            FirstName = "ValidFirstName", 
            LastName = "ValidLastName" 
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}