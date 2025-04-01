using FluentValidation;
using UserService.Application.DTO.Authentication;

namespace UserService.Application.Validation.Validators.Authentication;

public class UserLogoutDtoValidator : AbstractValidator<UserLogoutDto>
{
    private const int MaxUsernameLength = 100;

    public UserLogoutDtoValidator()
    {
        RuleFor(l => l.UserName)
            .NotEmpty()
            .WithMessage(l => ValidationUtils.EmptyParamMessage(nameof(l.UserName)))
            .MaximumLength(MaxUsernameLength)
            .WithMessage(l => ValidationUtils.TooLongParamMessage(nameof(l.UserName), MaxUsernameLength));
    }
}