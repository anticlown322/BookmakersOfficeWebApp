using BettingService.BLL.Exceptions.Base;

namespace BettingService.BLL.Exceptions.Specific;

public sealed class SportValidationFailedException(string rejectionReason)
    : BadRequestException($"Bet was cancelled. Reason:  {rejectionReason}");