using System.Text.Json;
using SportDataService.Application.Contracts.Services;

namespace SportDataService.Infrastructure.Services.Redis;

public class JsonRedisSerializer : ISerializer
{
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new PagedListConverterFactory() },
    };

    public byte[] Serialize<T>(T value)
        => JsonSerializer.SerializeToUtf8Bytes(value, _options);

    public T Deserialize<T>(byte[] value)
        => value == null ? default : JsonSerializer.Deserialize<T>(value, _options);
}