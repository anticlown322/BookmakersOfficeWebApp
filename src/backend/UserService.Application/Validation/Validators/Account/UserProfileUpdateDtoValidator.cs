using FluentValidation;
using UserService.Application.DTO.Account;

namespace UserService.Application.Validation.Validators.Account;

public class UserProfileUpdateDtoValidator : AbstractValidator<UserProfileUpdateDto>
{
    private const int MaxFirstNameLength = 100;
    private const int MaxLastNameLength = 100;

    public UserProfileUpdateDtoValidator()
    {
        RuleFor(u => u.FirstName)
            .NotEmpty()
            .WithMessage(u => ValidationUtils.EmptyParamMessage(nameof(u.FirstName)))
            .MaximumLength(MaxFirstNameLength)
            .WithMessage(u => ValidationUtils.TooLongParamMessage(nameof(u.FirstName), MaxFirstNameLength));

        RuleFor(u => u.LastName)
            .NotEmpty()
            .WithMessage(u => ValidationUtils.EmptyParamMessage(nameof(u.LastName)))
            .MaximumLength(MaxLastNameLength)
            .WithMessage(u => ValidationUtils.TooLongParamMessage(nameof(u.LastName), MaxLastNameLength));
    }
}