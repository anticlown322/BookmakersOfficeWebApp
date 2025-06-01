using Moq;
using Newtonsoft.Json.Linq;
using SportDataService.Domain.Models.Prematch;
using SportDataService.Domain.Models.Results;
using SportDataService.Infrastructure.Services.DataCollection;
using SportDataService.Infrastructure.Services.DataCollection.Abstractions;
using Match = SportDataService.Domain.Models.Prematch.Match;

namespace SportDataService.Tests.UnitTests.Services.DataCollection;

public class DataCollectionServiceTests
{
    private readonly Mock<IApiDataService> _apiDataServiceMock = new();
    private readonly Mock<IPrematchDataParser> _prematchParserMock = new();
    private readonly Mock<IResultsDataParser> _resultsParserMock = new();
    private readonly DataCollectionService _service;

    public DataCollectionServiceTests()
    {
        _service = new DataCollectionService(
            _apiDataServiceMock.Object,
            _prematchParserMock.Object,
            _resultsParserMock.Object);
    }

    [Fact]
    public async Task GetTournamentsInfoAsync_ShouldReturnTournaments_WhenDataIsValid()
    {
        // Arrange
        var rawData = JToken.Parse("{}");
        var expectedTournaments = new List<Tournament>
        {
            new() { TournamentId = "1", Name = "Tournament 1" }
        };
        var expectedMatches = new List<Match>
        {
            new() { MatchId = "1", TournamentId = "1" }
        };

        _apiDataServiceMock.Setup(x => x.GetMarketsDataAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(rawData);
        _prematchParserMock.Setup(x => x.ParseTournamentsPrematchData(rawData))
            .Returns(expectedTournaments);
        _prematchParserMock.Setup(x => x.ParseMatchesPrematchData(rawData))
            .Returns(expectedMatches);
        _prematchParserMock.Setup(x => x.LinkMatchesToTournaments(
                It.IsAny<List<Tournament>>(),
                It.IsAny<List<Match>>()))
            .Callback<List<Tournament>, List<Match>>((t, m) =>
            {
                var tournamentDict = t.ToDictionary(t => t.TournamentId);
                foreach (var match in m)
                {
                    if (tournamentDict.TryGetValue(match.TournamentId, out var tournament))
                    {
                        tournament.Matches.Add(match);
                    }
                }
            });

        // Act
        var result = await _service.GetTournamentsInfoAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull()
            .And.HaveCount(1)
            .And.ContainSingle(t => t.TournamentId == "1" && t.Name == "Tournament 1");

        result[0].Matches.Should().HaveCount(1)
            .And.ContainSingle(m => m.MatchId == "1");

        _prematchParserMock.Verify(x => x.LinkMatchesToTournaments(
            It.Is<List<Tournament>>(t => t.Count == 1),
            It.Is<List<Match>>(m => m.Count == 1)), Times.Once);
    }

    [Fact]
    public async Task GetTournamentsResultsInfoAsync_ShouldReturnTournamentResults_WhenDataIsValid()
    {
        // Arrange
        var rawData = JToken.Parse("{}");
        var expectedTournaments = new List<TournamentResult>
        {
            new() { TournamentId = "1", TournamentName = "Result Tournament 1" }
        };
        var expectedMatches = new List<MatchResult>
        {
            new() { MatchResultId = "1", TournamentId = "1" }
        };

        _apiDataServiceMock.Setup(x => x.GetResultsDataAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(rawData);
        _resultsParserMock.Setup(x => x.ParseTournamentResultsData(rawData))
            .Returns(expectedTournaments);
        _resultsParserMock.Setup(x => x.ParseMatchResultsData(rawData))
            .Returns(expectedMatches);
        _resultsParserMock.Setup(x => x.LinkMatchResultsToTournamentResults(
                It.IsAny<List<TournamentResult>>(),
                It.IsAny<List<MatchResult>>()))
            .Callback<List<TournamentResult>, List<MatchResult>>((tr, mr) =>
            {
                var tournamentDict = tr.ToDictionary(t => t.TournamentId);
                foreach (var matchResult in mr)
                {
                    if (tournamentDict.TryGetValue(matchResult.TournamentId, out var tournament))
                    {
                        tournament.Matches.Add(matchResult);
                    }
                }
            });

        // Act
        var result = await _service.GetTournamentsResultsInfoAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull()
            .And.HaveCount(1)
            .And.ContainSingle(t => t.TournamentId == "1" && t.TournamentName == "Result Tournament 1");

        result[0].Matches.Should().HaveCount(1)
            .And.ContainSingle(m => m.MatchResultId == "1");

        _resultsParserMock.Verify(x => x.LinkMatchResultsToTournamentResults(
            It.Is<List<TournamentResult>>(t => t.Count == 1),
            It.Is<List<MatchResult>>(m => m.Count == 1)), Times.Once);
    }

    [Fact]
    public async Task GetTournamentsInfoAsync_ShouldPropagateApiException_WhenApiFails()
    {
        // Arrange
        _apiDataServiceMock.Setup(x => x.GetMarketsDataAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("API error"));

        // Act & Assert
        await _service.Invoking(x => x.GetTournamentsInfoAsync(CancellationToken.None))
            .Should().ThrowAsync<HttpRequestException>()
            .WithMessage("API error");

        _prematchParserMock.Verify(x => x.ParseTournamentsPrematchData(It.IsAny<JToken>()), Times.Never);
    }

    [Fact]
    public async Task GetTournamentsResultsInfoAsync_ShouldUseCurrentDate_InApiRequest()
    {
        // Arrange
        var expectedDate = DateTime.Now.ToString("yyyy-MM-dd");
        var rawData = JToken.Parse("{}");

        _apiDataServiceMock.Setup(x => x.GetResultsDataAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(rawData)
            .Callback<CancellationToken>(ct =>
            {
                _apiDataServiceMock.Verify(x =>
                    x.GetResultsDataAsync(It.Is<CancellationToken>(c => c == ct)), Times.Once);
            });

        // Act
        await _service.GetTournamentsResultsInfoAsync(CancellationToken.None);

        // Assert
        _apiDataServiceMock.Verify(x => x.GetResultsDataAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetTournamentsInfoAsync_ShouldReturnEmptyList_WhenNoDataAvailable()
    {
        // Arrange
        var rawData = JToken.Parse("{}");

        _apiDataServiceMock.Setup(x => x.GetMarketsDataAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(rawData);
        _prematchParserMock.Setup(x => x.ParseTournamentsPrematchData(rawData))
            .Returns(new List<Tournament>());

        // Act
        var result = await _service.GetTournamentsInfoAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull().And.BeEmpty();
        _prematchParserMock.Verify(x => x.ParseMatchesPrematchData(rawData), Times.Once);
    }
}