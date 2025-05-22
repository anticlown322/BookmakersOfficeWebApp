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
        var model = new WithdrawRequestDto(0, "123456", null);
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Amount)
            .WithErrorMessage("Amount can't be empty.");
    }

    [Fact]
    public void Amount_ShouldHaveError_WhenLessThanMin()
    {
        var model = new WithdrawRequestDto(0.99m, "123456", null);
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Amount)
            .WithErrorMessage("Amount should be greater than 0,99 or equal to 0,99.");
    }

    [Fact]
    public void Amount_ShouldHaveError_WhenGreaterThanMax()
    {
        var model = new WithdrawRequestDto(10_000_001, "123456", null);
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Amount)
            .WithErrorMessage("Amount should be less than 10000001 or equal to 10000001.");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5_000_000)]
    [InlineData(10_000_000)]
    public void Amount_ShouldNotHaveError_WhenWithinRange(decimal amount)
    {
        var model = new WithdrawRequestDto(amount, "123456", null);
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Comment_ShouldHaveError_WhenExceedsMaxLength()
    {
        var longComment = new string('a', 256);
        var model = new WithdrawRequestDto(100, "123456", longComment);
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Comment)
            .WithErrorMessage("Comment can't be longer than 256 symbols.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Valid comment")]
    [InlineData("Short")]
    public void Comment_ShouldNotHaveError_WhenValid(string comment)
    {
        var model = new WithdrawRequestDto(100, "123456", comment);
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Comment);
    }

    [Fact]
    public void ConfirmationCode_ShouldHaveError_WhenNull()
    {
        var model = new WithdrawRequestDto(100, null, null);
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ConfirmationCode)
            .WithErrorMessage("ConfirmationCode can't be empty.");
    }

    [Fact]
    public void ConfirmationCode_ShouldHaveError_WhenEmpty()
    {
        var model = new WithdrawRequestDto(100, "", null);
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ConfirmationCode)
            .WithErrorMessage("ConfirmationCode can't be empty.");
    }

    [Theory]
    [InlineData("123456")]
    [InlineData("abcdef")]
    [InlineData("ABC123")]
    public void ConfirmationCode_ShouldNotHaveError_WhenValid(string code)
    {
        var model = new WithdrawRequestDto(100, code, null);
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.ConfirmationCode);
    }

    [Fact]
    public void ShouldNotHaveErrors_WhenAllFieldsValid()
    {
        var model = new WithdrawRequestDto(1000, "654321", "Valid comment");
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}