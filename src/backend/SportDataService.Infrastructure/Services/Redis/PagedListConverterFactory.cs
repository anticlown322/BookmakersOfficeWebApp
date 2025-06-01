using System.Text.Json;
using System.Text.Json.Serialization;
using SportDataService.Domain.RequestFeatures;

namespace SportDataService.Infrastructure.Services.Redis;

public class PagedListConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType &&
               typeToConvert.GetGenericTypeDefinition() == typeof(PagedList<>);
    }

    public override JsonConverter CreateConverter(Type type, JsonSerializerOptions options)
    {
        var itemType = type.GetGenericArguments()[0];
        var converterType = typeof(PagedListConverter<>).MakeGenericType(itemType);

        return (JsonConverter)Activator.CreateInstance(converterType);
    }
}