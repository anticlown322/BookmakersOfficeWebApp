using BettingService.DAL.Models.Entities;
using BettingService.DAL.RequestFeatures;
using BettingService.DAL.RequestFeatures.Params;

namespace BettingService.Tests.UnitTests.UseCases;

public class PayoutsUseCasesTestData
{
    public static List<Payout> GetTestPayouts()
    {
        return new List<Payout>
        {
            new Payout
            {
                Id = Guid.NewGuid(),
                BetId = Guid.NewGuid(),
                Amount = 100,
                Status = PayoutStatus.Completed,
                ProcessedAt = DateTime.UtcNow,
                ErrorReason = null
            },
            new Payout
            {
                Id = Guid.NewGuid(),
                BetId = Guid.NewGuid(),
                Amount = 200,
                Status = PayoutStatus.Failed,
                ProcessedAt = DateTime.UtcNow,
                ErrorReason = "Insufficient funds"
            }
        };
    }

    public static PagedList<Payout> GetTestPagedPayouts(PayoutParameters parameters)
    {
        var payouts = GetTestPayouts();
        return PagedList<Payout>.ToPagedList(
            payouts.AsQueryable(),
            parameters.PageNumber,
            parameters.PageSize);
    }
}