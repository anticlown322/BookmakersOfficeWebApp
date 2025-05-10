using System.Text.Json;
using System.Text.Json.Serialization;
using SportDataService.Domain.RequestFeatures;

namespace SportDataService.Infrastructure.Services.Redis;

public class PagedListConverter<T> : JsonConverter<PagedList<T>>
{
    public override PagedList<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        var items = JsonSerializer.Deserialize<List<T>>(root.GetProperty("Items").GetRawText(), options);
        var metaData = JsonSerializer.Deserialize<MetaData>(root.GetProperty("MetaData").GetRawText(), options);

        return new PagedList<T>(items, metaData.TotalCount, metaData.CurrentPage, metaData.PageSize);
    }

    public override void Write(Utf8JsonWriter writer, PagedList<T> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("Items");
        JsonSerializer.Serialize(writer, value.ToList(), options);

        writer.WritePropertyName("MetaData");
        JsonSerializer.Serialize(writer, value.MetaData, options);

        writer.WriteEndObject();
    }
}