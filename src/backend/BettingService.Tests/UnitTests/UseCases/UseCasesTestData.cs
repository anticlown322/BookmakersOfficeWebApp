using BettingService.DAL.Models.Entities;
using BettingService.DAL.RequestFeatures;
using BettingService.DAL.RequestFeatures.Params;

namespace BettingService.Tests.UnitTests.UseCases;

public static class UseCasesTestData
{
    public static Bet CreateTestBet(
        Guid? id = null,
        string username = "testUser",
        BetStatus status = BetStatus.Pending,
        string matchId = null,
        BetLineType lineType = BetLineType.Main,
        string marketSelection = "Team1")
    {
        return new Bet
        {
            Id = id ?? Guid.NewGuid(),
            Username = username,
            Status = status,
            MatchId = matchId ?? "match_" + Guid.NewGuid().ToString()[..8],
            LineType = lineType,
            MarketSelection = marketSelection,
            Amount = 100m,
            Odds = 1.85m,
            CreatedAt = DateTime.UtcNow.AddHours(-1)
        };
    }

    public static List<Bet> CreateTestUserBets(string username, int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => new Bet
            {
                Id = Guid.NewGuid(),
                Username = username,
                MatchId = $"match_{i}",
                LineType = (BetLineType)(i % 4),
                MarketSelection = $"Selection_{i}",
                Amount = i * 100m,
                Odds = 1.5m + (i * 0.1m),
                Status = (BetStatus)(i % 8),
                CreatedAt = DateTime.UtcNow.AddDays(-i),
                AcceptedAt = DateTime.UtcNow.AddDays(-i).AddMinutes(5)
            })
            .ToList();
    }
    
    public static List<Bet> CreateTestBetsWithStatus(int count, BetStatus status)
    {
        return Enumerable.Range(1, count)
            .Select(i => new Bet
            {
                Id = Guid.NewGuid(),
                Status = status,
                Username = $"user{i}",
                Amount = 100m * i,
                Odds = 1.5m + (0.1m * i),
                CreatedAt = DateTime.UtcNow.AddDays(-i)
            })
            .ToList();
    }
    
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