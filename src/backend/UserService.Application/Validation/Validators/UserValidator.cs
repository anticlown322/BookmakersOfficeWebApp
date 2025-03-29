using FluentValidation;

namespace UserService.Application.Validation.Validators;

public class UserValidator : AbstractValidator<Domain.Models.User>
{
    private const int MaxFirstNameLength = 100;
    private const int MaxLastNameLength = 100;

    public UserValidator()
    {

    }
}
