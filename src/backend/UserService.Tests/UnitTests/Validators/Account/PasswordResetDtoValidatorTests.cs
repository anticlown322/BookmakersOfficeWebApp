using FluentValidation.TestHelper;
using UserService.Application.DTO.Account;
using UserService.Application.Validation;
using UserService.Application.Validation.Validators.Account;

namespace UserService.Tests.UnitTests.Validators.Account;

public class PasswordResetDtoValidatorTests
{
    private readonly PasswordResetDtoValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenTokenIsEmpty()
    {
        // Arrange
        var model = new PasswordResetDto("", "ValidPass123");

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Token)
            .WithErrorMessage("Token can't be empty.");
    }

    [Fact]
    public void ShouldNotHaveError_WhenTokenIsValid()
    {
        // Arrange
        var model = new PasswordResetDto("valid_token", "ValidPass123");

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Token);
    }

    [Fact]
    public void ShouldHaveError_WhenNewPasswordIsEmpty()
    {
        // Arrange
        var model = new PasswordResetDto("valid_token", "");

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewPassword)
            .WithErrorMessage("NewPassword can't be empty.");
    }

    [Theory]
    [InlineData("short")]
    [InlineData("tooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooolong")]
    public void Should_Have_Error_When_NewPassword_Length_Is_Invalid(string password)
    {
        // Arrange
        var model = new PasswordResetDto("valid_token", password);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewPassword)
            .WithErrorMessage($"Password number must have length {ValidationUtils.MinPasswordLength} to {ValidationUtils.MaxPasswordLength}");
    }

    [Theory]
    [InlineData("ValidPass1")]
    [InlineData("ThisIsAValidPassword123")]
    [InlineData("12345678")]
    public void ShouldNotHaveErrorWhen_NewPasswordLengthIsValid(string password)
    {
        // Arrange
        var model = new PasswordResetDto("valid_token", password);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.NewPassword);
    }

    [Fact]
    public void ShouldNotHaveError_WhenAllFieldsAreValid()
    {
        // Arrange
        var model = new PasswordResetDto("valid_token", "ValidPass123");

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}