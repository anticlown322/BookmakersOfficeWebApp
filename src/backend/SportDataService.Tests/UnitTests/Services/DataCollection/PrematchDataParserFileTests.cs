using System.Reflection;
using Newtonsoft.Json.Linq;
using SportDataService.Infrastructure.Services.DataCollection.Implementations;

namespace SportDataService.Tests.UnitTests.Services.DataCollection;

public class PrematchDataParserFileTests
{
    private readonly PrematchDataParser _parser;

    public PrematchDataParserFileTests()
    {
        _parser = new PrematchDataParser();
    }
    
    private JToken LoadTestData(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"SportDataService.Tests.UnitTests.Services.DataCollection.TestData.{fileName}";
    
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new FileNotFoundException($"Resource {resourceName} not found");
        }
    
        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();
        return JToken.Parse(json);
    }

    [Fact]
    public void ParseTournamentsPrematchData_ShouldReturnTournaments_FromRealFile()
    {
        // Arrange
        var testData = LoadTestData("PrematchTestData.json");
        var expectedTournamentId = "78380";
        var expectedTournamentName = "Counter-Strike. ESL Impact League. Bo1";

        // Act
        var result = _parser.ParseTournamentsPrematchData(testData);

        // Assert
        result.Should().ContainSingle();
        result[0].TournamentId.Should().Be(expectedTournamentId);
        result[0].Name.Should().Be(expectedTournamentName);
    }

    [Fact]
    public void ParseMatchesPrematchData_ShouldReturnMatches_FromRealFile()
    {
        // Arrange
        var testData = LoadTestData("PrematchTestData.json");
        var expectedMatchId = "55403827";
        var expectedTeam1Name = "FURIA Female (ж)";
        var expectedTeam2Name = "DMS (ж)";

        // Act
        var result = _parser.ParseMatchesPrematchData(testData);

        // Assert
        result.Should().ContainSingle();
        result[0].MatchId.Should().Be(expectedMatchId);
        result[0].Opponent1.Name.Should().Be(expectedTeam1Name);
        result[0].Opponent2.Name.Should().Be(expectedTeam2Name);
    }

    [Fact]
    public void ProcessMarkets_ShouldSetCorrectOdds_FromRealFile()
    {
        // Arrange
        var testData = LoadTestData("PrematchTestData.json");
        var expectedOpponent1Win = "1,57";
        var expectedOpponent2Win = "2,25";

        // Act
        var matches = _parser.ParseMatchesPrematchData(testData);
        var match = matches.First();

        // Assert
        match.MainLine.Opponent1Win.Value.Should().Be(expectedOpponent1Win);
        match.MainLine.Opponent2Win.Value.Should().Be(expectedOpponent2Win);
    }
}