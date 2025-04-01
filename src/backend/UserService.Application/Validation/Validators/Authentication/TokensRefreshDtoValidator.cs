using FluentValidation;
using UserService.Application.DTO.Authentication;

namespace UserService.Application.Validation.Validators.Authentication;

public class TokensRefreshDtoValidator : AbstractValidator<TokensRefreshDto>
{
    public TokensRefreshDtoValidator()
    {
        RuleFor(l => l.RefreshToken)
            .NotEmpty()
            .WithMessage(l => ValidationUtils.EmptyParamMessage(nameof(l.RefreshToken)));

        RuleFor(l => l.AccessToken)
            .NotEmpty()
            .WithMessage(l => ValidationUtils.EmptyParamMessage(nameof(l.AccessToken)));
    }
}