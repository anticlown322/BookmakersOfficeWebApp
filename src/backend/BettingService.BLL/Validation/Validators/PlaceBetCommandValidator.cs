using BettingService.BLL.DTO.Bet;
using BettingService.BLL.UseCases.Bets.Commands.PlaceBet;

namespace BettingService.BLL.Validation.Validators;

using FluentValidation;

public class PlaceBetCommandValidator : AbstractValidator<PlaceBetCommand>
{
    public PlaceBetCommandValidator()
    {
        RuleFor(x => x.PlaceBetDto)
            .NotNull()
            .WithMessage(x => ValidationUtils.DtoDataRequired(nameof(x.PlaceBetDto)));

        When(x => x.PlaceBetDto != null, () =>
        {
            RuleFor(x => x.PlaceBetDto.MatchId)
                .NotEmpty()
                .WithMessage(x => ValidationUtils.EmptyParamMessage(nameof(x.PlaceBetDto.MatchId)))
                .Length(ValidationUtils.MatchIdMinLength, ValidationUtils.MatchIdMaxLength)
                .WithMessage(x =>
                    x.PlaceBetDto.MatchId.Length < ValidationUtils.MatchIdMinLength
                        ? ValidationUtils.TooShortParamMessage(nameof(x.PlaceBetDto.MatchId), ValidationUtils.MatchIdMinLength)
                        : ValidationUtils.TooLongParamMessage(nameof(x.PlaceBetDto.MatchId), ValidationUtils.MatchIdMaxLength));

            RuleFor(x => x.PlaceBetDto.Amount)
                .NotEmpty()
                .WithMessage(x => ValidationUtils.EmptyParamMessage(nameof(x.PlaceBetDto.Amount)))
                .GreaterThanOrEqualTo(ValidationUtils.MinAmount)
                .WithMessage(x => ValidationUtils.TooSmallValueParamMessage(
                    nameof(x.PlaceBetDto.Amount),
                    ValidationUtils.MinAmount))
                .LessThanOrEqualTo(ValidationUtils.MaxAmount)
                .WithMessage(x => ValidationUtils.TooLargeValueParamMessage(
                    nameof(x.PlaceBetDto.Amount),
                    ValidationUtils.MaxAmount));

            RuleFor(x => x.PlaceBetDto.Odds)
                .NotEmpty()
                .WithMessage(x => ValidationUtils.EmptyParamMessage(nameof(x.PlaceBetDto.Odds)))
                .GreaterThanOrEqualTo(ValidationUtils.MinOdds)
                .WithMessage(x => ValidationUtils.TooSmallValueParamMessage(
                    nameof(x.PlaceBetDto.Odds),
                    ValidationUtils.MinOdds))
                .LessThanOrEqualTo(ValidationUtils.MaxOdds)
                .WithMessage(x => ValidationUtils.TooLargeValueParamMessage(
                    nameof(x.PlaceBetDto.Odds),
                    ValidationUtils.MaxOdds));
        });
    }
}