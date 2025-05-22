using BettingService.Protos;

namespace BettingService.Tests.UnitTests.GrpcClients;

public static class GrpcClientsTestData
{
    public static GetUserBalanceResponse CreateValidBalanceResponse(double balance = 100.0)
    {
        return new GetUserBalanceResponse
        {
            Balance = balance,
            UserExists = true
        };
    }

    public static UpdateUserBalanceResponse CreateUpdateBalanceResponse(bool success = true, double newBalance = 150.0)
    {
        return new UpdateUserBalanceResponse
        {
            Success = success,
            NewBalance = newBalance
        };
    }
    
    public static ValidateBetResponse CreateValidBetResponse(bool isValid = true, double odds = 2.0)
    {
        return new ValidateBetResponse
        {
            IsValid = isValid,
            CurrentOdds = odds,
            IsLive = false
        };
    }

    public static GetMatchResultResponse CreateMatchResultResponse(string matchId, ResultStatus status)
    {
        return new GetMatchResultResponse
        {
            HasResult = true,
            Result = new MatchResultData
            {
                MatchId = matchId,
                Status = status,
                Team1TotalScore = 2,
                Team2TotalScore = 1
            }
        };
    }

    public static GetMatchResultsBatchResponse CreateBatchMatchResults(params string[] matchIds)
    {
        var response = new GetMatchResultsBatchResponse();
        foreach (var matchId in matchIds)
        {
            response.Results.Add(new MatchResultData
            {
                MatchId = matchId,
                Status = ResultStatus.Ended
            });
        }
        return response;
    }
}