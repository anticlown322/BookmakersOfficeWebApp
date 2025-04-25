using BettingService.BLL.Exceptions.Base;
using BettingService.DAL.Models.Entities;

namespace BettingService.BLL.Exceptions.Specific;

public sealed class PayoutAlreadyExistsException(Guid betId, PayoutStatus status)
    : BadRequestException($"Payout for bet with id {betId} already exists and has status {status.ToString()}.");