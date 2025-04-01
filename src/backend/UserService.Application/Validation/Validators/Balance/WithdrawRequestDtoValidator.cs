using FluentValidation;
using UserService.Application.DTO.Balance;

namespace UserService.Application.Validation.Validators.Balance;

public class WithdrawRequestDtoValidator : AbstractValidator<WithdrawRequestDto>
{
    public WithdrawRequestDtoValidator()
    {
        RuleFor(d => d.Amount)
            .NotEmpty()
            .WithMessage(d => ValidationUtils.EmptyParamMessage(nameof(d.Amount)))
            .GreaterThanOrEqualTo(ValidationUtils.MinBalanceAmount)
            .WithMessage(d => ValidationUtils.TooSmallValueParamMessage(nameof(d.Amount), d.Amount))
            .LessThanOrEqualTo(ValidationUtils.MaxBalanceAmount)
            .WithMessage(d => ValidationUtils.TooLargeValueParamMessage(nameof(d.Amount), d.Amount));

        RuleFor(d => d.Comment)
            .MaximumLength(ValidationUtils.MaxCommentLength)
            .WithMessage(d => ValidationUtils.TooLongParamMessage(nameof(d.Comment), d.Comment.Length));

        RuleFor(d => d.ConfirmationCode)
            .NotNull()
            .NotEmpty()
            .WithMessage(d => ValidationUtils.EmptyParamMessage(nameof(d.ConfirmationCode)));
    }
}