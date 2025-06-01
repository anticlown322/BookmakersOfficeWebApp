using BettingService.DAL.Models.Entities;

namespace BettingService.Tests.UnitTests.UseCases;

public class BetsUseCasesTestData
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
}