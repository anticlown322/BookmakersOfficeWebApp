using Google.Protobuf.WellKnownTypes;
using MongoDB.Bson;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.Models.Prematch;
using SportDataService.Domain.Models.Prematch.Lines;
using SportDataService.Domain.Models.Results;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.GrpcService.Exceptions;
using SportDataService.GrpcService.Utility;

namespace SportDataService.GrpcService.Services;

using Grpc.Core;

public class SportDataGrpcService(
    IMatchNoCacheRepository matchRepository,
    IMatchResultNoCacheRepository matchResultRepository)
    : SportDataService.SportDataServiceBase
{
    public override async Task<ValidateBetResponse> ValidateBet(
        ValidateBetRequest request,
        ServerCallContext context)
    {
        try
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            
            if (!ObjectId.TryParse(request.MatchId, out _))
            {
                throw new InvalidIdFormatException(request.MatchId);
            }

            var match = await matchRepository.GetByIdAsync(request.MatchId, CancellationToken.None);
            if (match == null)
            {
                throw new MatchNotFoundException(request.MatchId);   
            }

            var domainValue = GetCurrentOdds(match, request.LineType, request.MarketSelection);

            if (!double.TryParse(domainValue.Value, out var currentOdds))
            {
                throw new InvalidOddsFormatException(domainValue.Value);
            }

            if (Math.Abs(currentOdds - request.Odds) > 0.01)
            {
                throw new OddsChangedException(currentOdds);
            }

            var isMatchStarted = match.StartTime <= DateTime.UtcNow;
            if (isMatchStarted)
            {
                throw new MatchAlreadyStartedException(match.StartTime.Value);
            }

            return new ValidateBetResponse
            {
                IsValid = true,
                CurrentOdds = currentOdds,
                IsLive = isMatchStarted,
            };
        }
        catch (GrpcExceptionBase)
        {
            throw;
        }
        catch (Exception ex) when (ex is not RpcException)
        {
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }

    public override async Task<GetMatchResultResponse> GetMatchResult(GetMatchResultRequest request,
        ServerCallContext context)
    {
        var result = await matchResultRepository.GetMatchResultByMatchResultIdAsync(
            request.MatchId,
            context.CancellationToken);

        if (result == null)
        {
            return new GetMatchResultResponse { HasResult = false };
        }

        return new GetMatchResultResponse
        {
            HasResult = true,
            Result = ConvertToMatchResultData(result)
        };
    }

    public override async Task<GetMatchResultsBatchResponse> GetMatchResultsBatch(
        GetMatchResultsBatchRequest request,
        ServerCallContext context)
    {
        var results = await matchResultRepository.FindByConditionAsync(
            r => request.MatchIds.Contains(r.Id),
            context.CancellationToken);

        var response = new GetMatchResultsBatchResponse();
        foreach (var result in results)
        {
            response.Results.Add(ConvertToMatchResultData(result));
        }

        return response;
    }

    private MatchResultData ConvertToMatchResultData(MatchResult result)
    {
        var data = new MatchResultData
        {
            MatchId = result.Id,
            Status = (ResultStatus)result.Status,
            Team1 = new TeamData
            {
                Id = result.Team1.Id,
                Name = result.Team1.Name,
            },
            Team2 = new TeamData
            {
                Id = result.Team2.Id,
                Name = result.Team2.Name,
            },
            Team1TotalScore = result.Team1TotalScore,
            Team2TotalScore = result.Team2TotalScore,
            ResultTime = result.ResultTime.Value.ToTimestamp()
        };

        if (result.SubScores != null)
        {
            foreach (var subScore in result.SubScores)
            {
                data.SubScores.Add(new SubScoreData
                {
                    SubscorePosition = subScore.SubscorePosition,
                    Title = subScore.Title,
                    Team1Score = subScore.Team1Score,
                    Team2Score = subScore.Team2Score
                });
            }
        }

        if (result.EventResults != null)
        {
            foreach (var eventResult in result.EventResults)
            {
                var eventData = new MatchEventResultData
                {
                    EventName = eventResult.EventName,
                    Status = (ResultStatus)eventResult.Status,
                    Team1TotalScore = eventResult.Team1TotalScore,
                    Team2TotalScore = eventResult.Team2TotalScore
                };

                if (eventResult.SubScores != null)
                {
                    foreach (var subScore in eventResult.SubScores)
                    {
                        eventData.SubScores.Add(new SubScoreData
                        {
                            SubscorePosition = subScore.SubscorePosition,
                            Title = subScore.Title,
                            Team1Score = subScore.Team1Score,
                            Team2Score = subScore.Team2Score
                        });
                    }
                }

                data.EventResults.Add(eventData);
            }
        }

        if (result.Status == Domain.Models.Results.ResultStatus.Canceled)
        {
            data.Outcomes.Add("match_canceled", BetOutcomes.Canceled);
            return data;
        }

        if (result.Status != Domain.Models.Results.ResultStatus.Ended)
        {
            return data;
        }

        // Main
        data.Outcomes.Add(LineMarketTypes.Opponent1Win,
            DetermineMainOutcome(result, LineMarketTypes.Opponent1Win));
        data.Outcomes.Add(LineMarketTypes.Opponent2Win,
            DetermineMainOutcome(result, LineMarketTypes.Opponent2Win));
        data.Outcomes.Add(LineMarketTypes.Draw,
            DetermineMainOutcome(result, LineMarketTypes.Draw));
        data.Outcomes.Add(LineMarketTypes.Opponent1WinOrDraw,
            DetermineMainOutcome(result, LineMarketTypes.Opponent1WinOrDraw));
        data.Outcomes.Add(LineMarketTypes.Opponent2WinOrDraw,
            DetermineMainOutcome(result, LineMarketTypes.Opponent2WinOrDraw));

        // Kills
        data.Outcomes.Add(LineMarketTypes.Opponent1KillsMain,
            DetermineKillsOutcome(result, LineMarketTypes.Opponent1KillsMain));
        data.Outcomes.Add(LineMarketTypes.Opponent2KillsMain,
            DetermineKillsOutcome(result, LineMarketTypes.Opponent2KillsMain));
        data.Outcomes.Add(LineMarketTypes.TotalKillsUnder,
            DetermineKillsOutcome(result, LineMarketTypes.TotalKillsUnder));
        data.Outcomes.Add(LineMarketTypes.TotalKillsOver,
            DetermineKillsOutcome(result, LineMarketTypes.TotalKillsOver));
        data.Outcomes.Add(LineMarketTypes.Opponent1KillsHandicap,
            DetermineKillsOutcome(result, LineMarketTypes.Opponent1KillsHandicap));
        data.Outcomes.Add(LineMarketTypes.Opponent2KillsHandicap,
            DetermineKillsOutcome(result, LineMarketTypes.Opponent2KillsHandicap));

        // MapsLine
        data.Outcomes.Add(LineMarketTypes.Map1HandicapOpponent1, 
            DetermineMapsOutcome(result, LineMarketTypes.Map1HandicapOpponent1));
        data.Outcomes.Add(LineMarketTypes.Map1HandicapOpponent2, 
            DetermineMapsOutcome(result, LineMarketTypes.Map1HandicapOpponent2));
        data.Outcomes.Add(LineMarketTypes.Map2HandicapOpponent1, 
            DetermineMapsOutcome(result, LineMarketTypes.Map2HandicapOpponent1));
        data.Outcomes.Add(LineMarketTypes.Map2HandicapOpponent2, 
            DetermineMapsOutcome(result, LineMarketTypes.Map2HandicapOpponent2));

        // SpecialLine 
        data.Outcomes.Add(LineMarketTypes.EitherOpponent1OrOpponent2, 
            DetermineSpecialOutcome(result, LineMarketTypes.EitherOpponent1OrOpponent2));

        return data;
    }

    private string DetermineMainOutcome(MatchResult result, string marketSelection)
    {
        return marketSelection switch
        {
            LineMarketTypes.Opponent1Win =>
                result.Team1TotalScore > result.Team2TotalScore ? BetOutcomes.Won : BetOutcomes.Lost,

            LineMarketTypes.Opponent2Win =>
                result.Team2TotalScore > result.Team1TotalScore ? BetOutcomes.Won : BetOutcomes.Lost,

            LineMarketTypes.Draw =>
                result.Team1TotalScore == result.Team2TotalScore ? BetOutcomes.Won : BetOutcomes.Lost,

            LineMarketTypes.Opponent1WinOrDraw =>
                result.Team1TotalScore >= result.Team2TotalScore ? BetOutcomes.Won : BetOutcomes.Lost,

            LineMarketTypes.Opponent2WinOrDraw =>
                result.Team2TotalScore >= result.Team1TotalScore ? BetOutcomes.Won : BetOutcomes.Lost,

            _ => BetOutcomes.Refunded
        };
    }

    private string DetermineKillsOutcome(MatchResult result, string marketSelection)
    {
        var killsEvent = result.EventResults?.FirstOrDefault(e => e.EventName == EventTypes.Frags);
        if (killsEvent == null) return BetOutcomes.Refunded;

        return marketSelection switch
        {
            LineMarketTypes.Opponent1KillsMain =>
                killsEvent.Team1TotalScore > killsEvent.Team2TotalScore ? BetOutcomes.Won : BetOutcomes.Lost,

            LineMarketTypes.Opponent2KillsMain =>
                killsEvent.Team2TotalScore > killsEvent.Team1TotalScore ? BetOutcomes.Won : BetOutcomes.Lost,

            LineMarketTypes.TotalKillsUnder =>
                ParseTotalMarketAndCompare(killsEvent, marketSelection, (total, value) => total < value),

            LineMarketTypes.TotalKillsOver =>
                ParseTotalMarketAndCompare(killsEvent, marketSelection, (total, value) => total > value),

            LineMarketTypes.Opponent1KillsHandicap =>
                ParseHandicapMarketAndCompare(killsEvent, marketSelection, (t1, t2, h) => t1 - t2 + h > 0),

            LineMarketTypes.Opponent2KillsHandicap =>
                ParseHandicapMarketAndCompare(killsEvent, marketSelection, (t1, t2, h) => t2 - t1 + h > 0),

            _ => BetOutcomes.Refunded
        };
    }

    private string DetermineMapsOutcome(MatchResult result, string marketSelection)
    {
        try
        {
            var parts = marketSelection.Split('_');
            if (parts.Length < 3) return BetOutcomes.Refunded;

            var mapNumber = int.Parse(parts[0].Replace("Map", ""));
            var map = result.SubScores?.FirstOrDefault(s => s.SubscorePosition == mapNumber);
            if (map == null) return BetOutcomes.Refunded;

            var handicapValue = double.Parse(parts[2]);
            var isOpponent1 = parts[1].Contains("Opponent1");

            var diff = isOpponent1
                ? map.Team1Score - map.Team2Score + handicapValue
                : map.Team2Score - map.Team1Score + handicapValue;

            return diff > 0 ? BetOutcomes.Won : BetOutcomes.Lost;
        }
        catch
        {
            return BetOutcomes.Refunded;
        }
    }

    private string DetermineSpecialOutcome(MatchResult result, string marketSelection)
    {
        return marketSelection switch
        {
            LineMarketTypes.EitherOpponent1OrOpponent2 =>
                result.Team1TotalScore != result.Team2TotalScore ? BetOutcomes.Won : BetOutcomes.Lost,

            _ => BetOutcomes.Refunded
        };
    }

    private string ParseTotalMarketAndCompare(
        MatchEventResult fragsEvent,
        string marketSelection,
        Func<double, double, bool> compareFunc)
    {
        try
        {
            var parts = marketSelection.Split('_');
            if (parts.Length < 2) return BetOutcomes.Refunded;

            var totalValue = double.Parse(parts[1]);
            var totalFrags = fragsEvent.Team1TotalScore + fragsEvent.Team2TotalScore;

            return compareFunc(totalFrags, totalValue) ? BetOutcomes.Won : BetOutcomes.Lost;
        }
        catch
        {
            return BetOutcomes.Refunded;
        }
    }

    private string ParseHandicapMarketAndCompare(
        MatchEventResult fragsEvent,
        string marketSelection,
        Func<double, double, double, bool> compareFunc)
    {
        try
        {
            var parts = marketSelection.Split('_');
            if (parts.Length < 2) return BetOutcomes.Refunded;

            var handicapValue = double.Parse(parts[1]);
            return compareFunc(
                fragsEvent.Team1TotalScore,
                fragsEvent.Team2TotalScore,
                handicapValue)
                ? BetOutcomes.Won
                : BetOutcomes.Lost;
        }
        catch
        {
            return BetOutcomes.Refunded;
        }
    }

    private Domain.Models.Prematch.Markets.MarketValue GetCurrentOdds(Match match, string lineType,
        string marketSelection)
    {
        IMarketLine line = lineType switch
        {
            LineTypes.MainLine => match.MainLine,
            LineTypes.KillsLine => match.KillsLine,
            LineTypes.MapsLine => match.MapsLine,
            LineTypes.SpecialLine => match.SpecialLine,
            _ => throw new InvalidLineTypeException(lineType)
        };

        var value = line.GetValue(marketSelection);

        if (value == null || !value.IsActive)
        {
            throw new MarketNotFoundException(marketSelection);   
        }

        return value;
    }
}