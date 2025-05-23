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
        // Arrange
        var model = new TokensRefreshDto("validAccessToken", "");
        
        // Act
        var result = _validator.TestValidate(model);
        
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RefreshToken);
    }

    [Fact]
    public void AccessToken_ShouldHaveError_WhenEmpty()
    {
        // Arrange
        var model = new TokensRefreshDto("", "validRefreshToken");
        
        // Act
        var result = _validator.TestValidate(model);
        
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AccessToken);
    }

    [Theory]
    [InlineData("token1", "token2")]
    [InlineData("refresh123", "access123")]
    public void ShouldNotHaveErrors_WhenTokensValid(string refreshToken, string accessToken)
    {
        // Arrange
        var model = new TokensRefreshDto(refreshToken, accessToken);
        
        // Act
        var result = _validator.TestValidate(model);
        
        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}