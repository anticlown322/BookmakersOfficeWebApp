using System.Reflection;
using Newtonsoft.Json.Linq;
using SportDataService.Infrastructure.Services.DataCollection.Implementations;

namespace SportDataService.Tests.UnitTests.Services.DataCollection;

public class ResultsDataParserTests
{
    private readonly ResultsDataParser _parser;

    public ResultsDataParserTests()
    {
        _parser = new ResultsDataParser();
    }

    private JToken LoadTestData()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "SportDataService.Tests.UnitTests.Services.DataCollection.TestData.ResultsTestData.json";
        
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            var availableResources = string.Join(", ", assembly.GetManifestResourceNames());
            throw new FileNotFoundException(
                $"Resource '{resourceName}' not found. Available resources: {availableResources}");
        }
        
        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();
        return JToken.Parse(json);
    }

    [Fact]
    public void ParseTournamentResultsData_ShouldReturnTournaments_FromEmbeddedResource()
    {
        // Arrange
        var testData = LoadTestData();
        var expectedTournamentId = "100305";
        var expectedTournamentName = "Counter-Strike. H2H CS. 2X2. Москва. Bo1";

        // Act
        var result = _parser.ParseTournamentResultsData(testData);

        // Assert
        result.Should().ContainSingle();
        result[0].TournamentId.Should().Be(expectedTournamentId);
        result[0].TournamentName.Should().Be(expectedTournamentName);
    }

    [Fact]
    public void ParseMatchResultsData_ShouldReturnMatches_FromEmbeddedResource()
    {
        // Arrange
        var testData = LoadTestData();
        var expectedMatch1Id = "55544495";
        var expectedMatch2Id = "55544494";
        var expectedTeam1Name = "NEO-NOIR BROS";
        var expectedTeam2Name = "ASIIMOV BOYS";

        // Act
        var result = _parser.ParseMatchResultsData(testData);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(m => m.MatchResultId == expectedMatch1Id);
        result.Should().Contain(m => m.MatchResultId == expectedMatch2Id);
        result[0].Team1.Name.Should().Be(expectedTeam1Name);
        result[0].Team2.Name.Should().Be(expectedTeam2Name);
    }

    [Fact]
    public void ParseMatchResultsData_ShouldSetCorrectScores_FromEmbeddedResource()
    {
        // Arrange
        var testData = LoadTestData();
        var expectedFirstMatchScore1 = 7;
        var expectedFirstMatchScore2 = 9;

        // Act
        var result = _parser.ParseMatchResultsData(testData);
        var firstMatch = result.First();

        // Assert
        firstMatch.Team1TotalScore.Should().Be(expectedFirstMatchScore1);
        firstMatch.Team2TotalScore.Should().Be(expectedFirstMatchScore2);
    }

    [Fact]
    public void LinkMatchResultsToTournamentResults_ShouldLinkCorrectly_FromEmbeddedResource()
    {
        // Arrange
        var testData = LoadTestData();
        var tournaments = _parser.ParseTournamentResultsData(testData);
        var matches = _parser.ParseMatchResultsData(testData);

        // Act
        _parser.LinkMatchResultsToTournamentResults(tournaments, matches);

        // Assert
        tournaments[0].Matches.Should().HaveCount(2);
        tournaments[0].Matches[0].MatchResultId.Should().Be("55544495");
        tournaments[0].Matches[1].MatchResultId.Should().Be("55544494");
    }
}