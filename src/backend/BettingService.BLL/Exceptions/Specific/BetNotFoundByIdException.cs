using BettingService.BLL.Exceptions.Base;

namespace BettingService.BLL.Exceptions.Specific;

public sealed class BetNotFoundByIdException(Guid id)
    : NotFoundException($"The bet with id: {id} does not exist in the database.");