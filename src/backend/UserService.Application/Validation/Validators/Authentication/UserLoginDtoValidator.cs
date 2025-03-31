using FluentValidation;
using UserService.Application.DTO.Authentication;

namespace UserService.Application.Validation.Validators.Authentication;

public class UserLoginDtoValidator : AbstractValidator<UserLoginDto>
{
    private const int MaxUsernameLength = 100;
    private const int MaxPasswordLength = 100;

    public UserLoginDtoValidator()
    {
        RuleFor(l => l.UserName)
            .NotEmpty()
            .WithMessage(l => ValidationUtils.EmptyParamMessage(nameof(l.UserName)))
            .MaximumLength(MaxUsernameLength)
            .WithMessage(l => ValidationUtils.TooLongParamMessage(nameof(l.UserName), MaxUsernameLength));

        RuleFor(l => l.Password)
            .NotEmpty()
            .WithMessage(l => ValidationUtils.EmptyParamMessage(nameof(l.Password)))
            .MaximumLength(MaxPasswordLength)
            .WithMessage(l => ValidationUtils.TooLongParamMessage(nameof(l.Password), MaxPasswordLength));

    }
}