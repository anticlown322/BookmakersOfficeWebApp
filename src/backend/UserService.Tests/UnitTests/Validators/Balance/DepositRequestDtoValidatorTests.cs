using FluentValidation.TestHelper;
using UserService.Application.DTO.Balance;
using UserService.Application.Validation;
using UserService.Application.Validation.Validators.Balance;

namespace UserService.Tests.UnitTests.Validators.Balance;

public class DepositRequestDtoValidatorTests
{
    private readonly DepositRequestDtoValidator _validator = new();

    [Fact]
    public void Amount_ShouldHaveError_WhenEmpty()
    {
        // Arrange
        var model = new DepositRequestDto(0, "Valid comment");

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Amount_ShouldHaveError_WhenLessThanMin()
    {
        // Arrange
        var model = new DepositRequestDto(ValidationUtils.MinBalanceAmount - 1, "Valid comment");

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Amount_ShouldHaveError_WhenGreaterThanMax()
    {
        // Arrange
        var model = new DepositRequestDto(ValidationUtils.MaxBalanceAmount + 1, "Valid comment");

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5_000_000)]
    [InlineData(10_000_000)]
    public void Amount_ShouldNotHaveError_WhenWithinRange(decimal amount)
    {
        // Arrange
        var model = new DepositRequestDto(amount, "Valid comment");

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Comment_ShouldHaveError_WhenExceedsMaxLength()
    {
        // Arrange
        var longComment = new string('a', ValidationUtils.MaxCommentLength + 1);
        var model = new DepositRequestDto(100, longComment);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Comment);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Valid comment")]
    [InlineData("Very very very long comment but still under limit")]
    public void Comment_ShouldNotHaveError_WhenValid(string comment)
    {
        // Arrange
        var model = new DepositRequestDto(100, comment);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Comment);
    }

    [Fact]
    public void ShouldNotHaveErrors_WhenAllFieldsValid()
    {
        // Arrange
        var model = new DepositRequestDto(1000, "Valid comment");

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}