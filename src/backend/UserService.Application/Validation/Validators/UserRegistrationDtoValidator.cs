using FluentValidation;
using UserService.Application.DTO.Authentication;

namespace UserService.Application.Validation.Validators;

public class UserRegistrationDtoValidator : AbstractValidator<UserForRegistrationDto>
{
    private const int MaxFirstNameLength = 100;
    private const int MaxLastNameLength = 100;
    private const int MaxLoginLength = 100;
    private const int MaxPasswordLength = 100;
    private const int MinPhoneNumberLength = 10;
    private const int MaxPhoneNumberLength = 15;

    public UserRegistrationDtoValidator()
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

        RuleFor(l => l.UserName)
            .NotEmpty()
            .WithMessage(l => ValidationUtils.EmptyParamMessage(nameof(l.UserName)))
            .MaximumLength(MaxLoginLength)
            .WithMessage(l => ValidationUtils.TooLongParamMessage(nameof(l.UserName), MaxLoginLength));

        RuleFor(l => l.Password)
            .NotEmpty()
            .WithMessage(l => ValidationUtils.EmptyParamMessage(nameof(l.Password)))
            .MaximumLength(MaxPasswordLength)
            .WithMessage(l => ValidationUtils.TooLongParamMessage(nameof(l.Password), MaxPasswordLength));

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
            .Length(MinPhoneNumberLength, MaxPhoneNumberLength)
            .WithMessage($"Phone number must have length {MinPhoneNumberLength} to {MaxPhoneNumberLength}");
    }
}