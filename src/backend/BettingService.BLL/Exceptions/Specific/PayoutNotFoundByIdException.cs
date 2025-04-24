using BettingService.BLL.Exceptions.Base;

namespace BettingService.BLL.Exceptions.Specific;

public sealed class PayoutNotFoundByIdException(Guid id)
    : NotFoundException($"The payout with id: {id} does not exist in the database.");