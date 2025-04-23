using BettingService.BLL.DTO.Bet;

namespace BettingService.BLL.Validation.Validators;

using FluentValidation;

public class PlaceBetDtoValidator : AbstractValidator<PlaceBetDto>
{
    public PlaceBetDtoValidator()
    {
        RuleFor(x => x.MatchId)
            .NotEmpty()
            .WithMessage(x => ValidationUtils.EmptyParamMessage(nameof(x.MatchId)))
            .Length(ValidationUtils.MatchIdMinLength, ValidationUtils.MatchIdMaxLength)
            .WithMessage(x =>
                x.MatchId.Length < ValidationUtils.MatchIdMinLength
                    ? ValidationUtils.TooShortParamMessage(nameof(x.MatchId), ValidationUtils.MatchIdMinLength)
                    : ValidationUtils.TooLongParamMessage(nameof(x.MatchId), ValidationUtils.MatchIdMaxLength));

        RuleFor(x => x.Amount)
            .NotEmpty()
            .WithMessage(x => ValidationUtils.EmptyParamMessage(nameof(x.Amount)))
            .GreaterThanOrEqualTo(ValidationUtils.MinAmount)
            .WithMessage(x => ValidationUtils.TooSmallValueParamMessage(nameof(x.Amount), ValidationUtils.MinAmount))
            .LessThanOrEqualTo(ValidationUtils.MaxAmount)
            .WithMessage(x => ValidationUtils.TooLargeValueParamMessage(nameof(x.Amount), ValidationUtils.MaxAmount));

        RuleFor(x => x.Odds)
            .NotEmpty()
            .WithMessage(x => ValidationUtils.EmptyParamMessage(nameof(x.Odds)))
            .GreaterThanOrEqualTo(ValidationUtils.MinOdds)
            .WithMessage(x => ValidationUtils.TooSmallValueParamMessage(nameof(x.Odds), ValidationUtils.MinOdds))
            .LessThanOrEqualTo(ValidationUtils.MaxOdds)
            .WithMessage(x => ValidationUtils.TooLargeValueParamMessage(nameof(x.Odds), ValidationUtils.MaxOdds));
    }
}