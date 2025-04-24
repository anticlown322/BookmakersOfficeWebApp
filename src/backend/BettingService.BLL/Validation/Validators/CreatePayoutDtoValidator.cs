using BettingService.BLL.DTO.Payout;

namespace BettingService.BLL.Validation.Validators;

using FluentValidation;

public class CreatePayoutDtoValidator : AbstractValidator<CreatePayoutDto>
{
    public CreatePayoutDtoValidator()
    {
        RuleFor(x => x.BetId)
            .NotEmpty()
            .WithMessage(x => ValidationUtils.EmptyParamMessage(nameof(x.BetId)));

        RuleFor(x => x.Amount)
            .NotEmpty()
            .WithMessage(x => ValidationUtils.EmptyParamMessage(nameof(x.Amount)))
            .GreaterThanOrEqualTo(ValidationUtils.MinAmount)
            .WithMessage(x => ValidationUtils.TooSmallValueParamMessage(nameof(x.Amount), ValidationUtils.MinAmount))
            .LessThanOrEqualTo(ValidationUtils.MaxAmount)
            .WithMessage(x => ValidationUtils.TooLargeValueParamMessage(nameof(x.Amount), ValidationUtils.MaxAmount));
    }
}