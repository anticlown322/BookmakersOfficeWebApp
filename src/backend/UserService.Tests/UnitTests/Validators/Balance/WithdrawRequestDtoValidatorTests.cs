using FluentValidation.TestHelper;
using UserService.Application.DTO.Balance;
using UserService.Application.Validation.Validators.Balance;

namespace UserService.Tests.UnitTests.Validators.Balance;

public class WithdrawRequestDtoValidatorTests
{
    private readonly WithdrawRequestDtoValidator _validator = new();

    [Fact]
    public void Amount_ShouldHaveError_WhenEmpty()
    {
        // Arrange
        var model = new WithdrawRequestDto(0, "123456", null);
        
        // Act
        var result = _validator.TestValidate(model);
        
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Amount_ShouldHaveError_WhenLessThanMin()
    {
        // Arrange
        var model = new WithdrawRequestDto(0.99m, "123456", null);
        
        // Act
        var result = _validator.TestValidate(model);
        
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Amount_ShouldHaveError_WhenGreaterThanMax()
    {
        // Arrange
        var model = new WithdrawRequestDto(10_000_001, "123456", null);
        
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
        var model = new WithdrawRequestDto(amount, "123456", null);
        
        // Act
        var result = _validator.TestValidate(model);
        
        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Comment_ShouldHaveError_WhenExceedsMaxLength()
    {
        // Arrange
        var longComment = new string('a', 256);
        var model = new WithdrawRequestDto(100, "123456", longComment);
        
        // Act
        var result = _validator.TestValidate(model);
        
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Comment);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Valid comment")]
    [InlineData("Short")]
    public void Comment_ShouldNotHaveError_WhenValid(string comment)
    {
        // Arrange
        var model = new WithdrawRequestDto(100, "123456", comment);
        
        // Act
        var result = _validator.TestValidate(model);
        
        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Comment);
    }

    [Fact]
    public void ConfirmationCode_ShouldHaveError_WhenNull()
    {
        // Arrange
        var model = new WithdrawRequestDto(100, null, null);
        
        // Act
        var result = _validator.TestValidate(model);
        
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ConfirmationCode);
    }

    [Fact]
    public void ConfirmationCode_ShouldHaveError_WhenEmpty()
    {
        // Arrange
        var model = new WithdrawRequestDto(100, "", null);
        
        // Act
        var result = _validator.TestValidate(model);
        
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ConfirmationCode);
    }

    [Theory]
    [InlineData("123456")]
    [InlineData("abcdef")]
    [InlineData("ABC123")]
    public void ConfirmationCode_ShouldNotHaveError_WhenValid(string code)
    {
        // Arrange
        var model = new WithdrawRequestDto(100, code, null);
        
        // Act
        var result = _validator.TestValidate(model);
        
        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ConfirmationCode);
    }

    [Fact]
    public void ShouldNotHaveErrors_WhenAllFieldsValid()
    {
        // Arrange
        var model = new WithdrawRequestDto(1000, "654321", "Valid comment");
        
        // Act
        var result = _validator.TestValidate(model);
        
        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}