using Newtonsoft.Json.Linq;
using SportDataService.Infrastructure.Services.DataCollection.Helpers;

namespace SportDataService.Tests.UnitTests.Services.DataCollection;

public class DateTimeParserTests
{
    [Fact]
    public void ParseDateTime_ShouldReturnNull_WhenTokenIsEmpty()
    {
        // Arrange
        var emptyToken = JToken.Parse("\"\"");

        // Act
        var result = ParsingUtils.ParseDateTime(emptyToken);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ParseDateTime_ShouldReturnNull_WhenTokenIsInvalid()
    {
        // Arrange
        var invalidToken = JToken.Parse("\"invalid\"");

        // Act
        var result = ParsingUtils.ParseDateTime(invalidToken);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ParseDateTime_ShouldReturnCorrectDateTime_WhenTokenIsValidUnixTimestamp()
    {
        // Arrange
        var unixTimestampToken = JToken.Parse("1640995200"); // 2022-01-01 00:00:00 UTC
        var expectedDateTime = new DateTime(2022, 1, 1, 3, 0, 0, DateTimeKind.Unspecified);

        // Act
        var result = ParsingUtils.ParseDateTime(unixTimestampToken);

        // Assert
        result.Should().Be(expectedDateTime);
    }

    [Fact]
    public void ParseDateTime_ShouldReturnUtcPlus3_WhenTokenIsValidUnixTimestamp()
    {
        // Arrange
        var _unixTimestampToken = JToken.Parse("1640995200"); // 2022-01-01 00:00:00 UTC
        var expectedHour = 3;

        // Act
        var result = ParsingUtils.ParseDateTime(_unixTimestampToken);

        // Assert
        result?.Hour.Should().Be(expectedHour);
    }
}