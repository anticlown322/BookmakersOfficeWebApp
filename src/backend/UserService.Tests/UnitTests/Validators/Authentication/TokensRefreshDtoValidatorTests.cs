using FluentValidation.TestHelper;
using UserService.Application.DTO.Authentication;
using UserService.Application.Validation.Validators.Authentication;

namespace UserService.Tests.UnitTests.Validators.Authentication;

public class TokensRefreshDtoValidatorTests
{
    private readonly TokensRefreshDtoValidator _validator = new();

    [Fact]
    public void RefreshToken_ShouldHaveError_WhenEmpty()
    {
        var model = new TokensRefreshDto("validAccessToken", "");
        var result = _validator.TestValidate(model);
        
        result.ShouldHaveValidationErrorFor(x => x.RefreshToken)
            .WithErrorMessage("RefreshToken can't be empty.");
    }

    [Fact]
    public void AccessToken_ShouldHaveError_WhenEmpty()
    {
        var model = new TokensRefreshDto("", "validRefreshToken");
        var result = _validator.TestValidate(model);
        
        result.ShouldHaveValidationErrorFor(x => x.AccessToken)
            .WithErrorMessage("AccessToken can't be empty.");
    }

    [Theory]
    [InlineData("token1", "token2")]
    [InlineData("refresh123", "access123")]
    public void ShouldNotHaveErrors_WhenTokensValid(string refreshToken, string accessToken)
    {
        var model = new TokensRefreshDto(refreshToken, accessToken);
        var result = _validator.TestValidate(model);
        
        result.ShouldNotHaveAnyValidationErrors();
    }
}