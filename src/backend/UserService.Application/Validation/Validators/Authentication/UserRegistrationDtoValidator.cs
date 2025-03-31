using FluentValidation;
using UserService.Application.DTO.Authentication;

namespace UserService.Application.Validation.Validators.Authentication;

public class UserRegistrationDtoValidator : AbstractValidator<UserRegistrationDto>
{
    public UserRegistrationDtoValidator()
    {
        RuleFor(u => u.FirstName)
            .NotEmpty()
            .WithMessage(u => ValidationUtils.EmptyParamMessage(nameof(u.FirstName)))
            .MaximumLength(ValidationUtils.MaxFirstNameLength)
            .WithMessage(u => ValidationUtils.TooLongParamMessage(nameof(u.FirstName), ValidationUtils.MaxFirstNameLength));

        RuleFor(u => u.LastName)
            .NotEmpty()
            .WithMessage(u => ValidationUtils.EmptyParamMessage(nameof(u.LastName)))
            .MaximumLength(ValidationUtils.MaxLastNameLength)
            .WithMessage(u => ValidationUtils.TooLongParamMessage(nameof(u.LastName), ValidationUtils.MaxLastNameLength));

        RuleFor(l => l.UserName)
            .NotEmpty()
            .WithMessage(l => ValidationUtils.EmptyParamMessage(nameof(l.UserName)))
            .MaximumLength(ValidationUtils.MaxUsernameLength)
            .WithMessage(l => ValidationUtils.TooLongParamMessage(nameof(l.UserName), ValidationUtils.MaxUsernameLength));

        RuleFor(l => l.Password)
            .NotEmpty()
            .WithMessage(l => ValidationUtils.EmptyParamMessage(nameof(l.Password)))
            .Length(ValidationUtils.MinPasswordLength, ValidationUtils.MaxPasswordLength)
            .WithMessage($"Password number must have length {ValidationUtils.MinPasswordLength} to {ValidationUtils.MaxPasswordLength}");

        RuleFor(c => c.Email)
            .NotEmpty()
            .WithMessage(c => ValidationUtils.EmptyParamMessage(nameof(c.Email)))
            .EmailAddress()
            .WithMessage("Invalid email address.");

        RuleFor(c => c.PhoneNumber)
            .NotEmpty()
            .WithMessage(c => ValidationUtils.EmptyParamMessage(nameof(c.PhoneNumber)))
            .Matches(@"^\+?[0-9]{10,15}$")
            .WithMessage("Invalid phone number.")
            .Length(ValidationUtils.MinPhoneNumberLength, ValidationUtils.MaxPhoneNumberLength)
            .WithMessage($"Phone number must have length {ValidationUtils.MinPhoneNumberLength} to {ValidationUtils.MaxPhoneNumberLength}");
    }
}