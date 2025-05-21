using Newtonsoft.Json.Linq;

namespace SportDataService.Infrastructure.Services.DataCollection.Helpers;

public static class ParsingUtils
{
    public static DateTime? ParseDateTime(JToken timeToken)
    {
        if (string.IsNullOrWhiteSpace(timeToken.ToString()))
        {
            return null;
        }

        if (long.TryParse(timeToken.ToString(), out var unixTimestamp))
        {
            // add 3 hours for UTC + 3 time (Moscow / Minsk)
            var dateTime = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp)
                .UtcDateTime
                .AddHours(3);

            return dateTime;
        }

        return null;
    }
}