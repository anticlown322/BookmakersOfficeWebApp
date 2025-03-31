using FluentValidation;
using UserService.Application.DTO.Account;

namespace UserService.Application.Validation.Validators.Account;

public class PasswordResetDtoValidator : AbstractValidator<PasswordResetDto>
{
    public PasswordResetDtoValidator()
    {
        RuleFor(d => d.Token)
            .NotEmpty()
            .WithMessage(l => ValidationUtils.EmptyParamMessage(nameof(l.Token)));

        RuleFor(l => l.NewPassword)
            .NotEmpty()
            .WithMessage(l => ValidationUtils.EmptyParamMessage(nameof(l.NewPassword)))
            .Length(ValidationUtils.MinPasswordLength, ValidationUtils.MaxPasswordLength)
            .WithMessage($"Password number must have length {ValidationUtils.MinPasswordLength} to {ValidationUtils.MaxPasswordLength}");
    }
}