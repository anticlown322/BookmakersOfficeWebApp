using FluentValidation;

namespace UserService.Application.Validation.Validators;

public class UserValidator : AbstractValidator<Domain.Models.User>
{
    private const int MaxFirstNameLength = 100;
    private const int MaxLastNameLength = 100;

    public UserValidator()
    {
        RuleFor(u => u.FirstName)
            .NotEmpty()
            .WithMessage(u => ValidationUtils.EmptyParamMessage(nameof(u.FirstName)))
            .MaximumLength(MaxFirstNameLength)
            .WithMessage(u => ValidationUtils.TooLongParamMessage(nameof(u.UserName), MaxFirstNameLength));

        RuleFor(u => u.LastName)
            .NotEmpty()
            .WithMessage(u => ValidationUtils.EmptyParamMessage(nameof(u.LastName)))
            .MaximumLength(MaxLastNameLength)
            .WithMessage(u => ValidationUtils.TooLongParamMessage(nameof(u.LastName), MaxLastNameLength));
    }
}
