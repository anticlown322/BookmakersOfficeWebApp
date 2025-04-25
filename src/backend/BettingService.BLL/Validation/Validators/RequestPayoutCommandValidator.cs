using BettingService.BLL.UseCases.Payouts.Commands.RequestPayout;

namespace BettingService.BLL.Validation.Validators;

using FluentValidation;

public class RequestPayoutCommandValidator : AbstractValidator<RequestPayoutCommand>
{
    public RequestPayoutCommandValidator()
    {
        RuleFor(x => x.RequestPayoutDto)
            .NotNull()
            .WithMessage(x => ValidationUtils.DtoDataRequired(nameof(x.RequestPayoutDto)));

        When(
            x => x.RequestPayoutDto != null,
            () =>
            {
                RuleFor(x => x.RequestPayoutDto.BetId)
                    .NotEmpty()
                    .WithMessage(x => ValidationUtils.EmptyParamMessage(nameof(x.RequestPayoutDto.BetId)))
                    .Must(x => ValidationUtils.BeValidGuidString(x.ToString()))
                    .WithMessage(x => ValidationUtils.InvalidGuidValue(
                        nameof(x.RequestPayoutDto.BetId),
                        x.RequestPayoutDto.BetId));

                RuleFor(x => x.RequestPayoutDto.Amount)
                    .NotEmpty()
                    .WithMessage(x => ValidationUtils.EmptyParamMessage(nameof(x.RequestPayoutDto.Amount)))
                    .GreaterThanOrEqualTo(ValidationUtils.MinAmount)
                    .WithMessage(x => ValidationUtils.TooSmallValueParamMessage(
                        nameof(x.RequestPayoutDto.Amount),
                        ValidationUtils.MinAmount))
                    .LessThanOrEqualTo(ValidationUtils.MaxAmount)
                    .WithMessage(x => ValidationUtils.TooLargeValueParamMessage(
                        nameof(x.RequestPayoutDto.Amount),
                        ValidationUtils.MaxAmount));
            });
    }
}