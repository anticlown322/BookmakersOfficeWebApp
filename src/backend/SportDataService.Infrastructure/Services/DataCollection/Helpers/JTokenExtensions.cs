using Newtonsoft.Json.Linq;

namespace SportDataService.Infrastructure.Services.DataCollection.Helpers;

public static class JTokenExtensions
{
    public static string GetStringValue(this JToken token, string fieldName)
    {
        return token[fieldName]?.ToString();
    }

    public static List<JToken> GetList(this JToken token, string fieldName)
    {
        return token[fieldName]?.ToList() ?? new List<JToken>();
    }
}