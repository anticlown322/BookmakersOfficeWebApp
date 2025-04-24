using BettingService.BLL.Exceptions.Base;

namespace BettingService.BLL.Exceptions.Specific;

public sealed class PayoutNotFoundByBetIdException(Guid id)
    : NotFoundException($"The payout with bet id: {id} does not exist in the database.");