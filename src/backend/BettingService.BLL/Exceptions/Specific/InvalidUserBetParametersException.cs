using BettingService.BLL.Exceptions.Base;

namespace BettingService.BLL.Exceptions.Specific;

public sealed class InvalidBetParametersException()
    : BadRequestException("Invalid bet parameters.");