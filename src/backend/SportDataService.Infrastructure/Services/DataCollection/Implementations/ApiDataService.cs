using System.IO.Compression;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SportDataService.Domain.Models.Settings;
using SportDataService.Infrastructure.Services.DataCollection.Abstractions;

namespace SportDataService.Infrastructure.Services.DataCollection.Implementations;

public class ApiDataService : IApiDataService
{
    private readonly HttpClient _httpClient;
    private readonly DataCollectionServiceSettings _settings;

    public ApiDataService(HttpClient httpClient, IOptions<DataCollectionServiceSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
    }

    public async Task<JToken> GetMarketsDataAsync(CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(_settings.MarketsUrl, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        await using var decompressedStream = new GZipStream(responseStream, CompressionMode.Decompress);

        using var jsonReader = new StreamReader(decompressedStream);
        using var jsonTextReader = new JsonTextReader(jsonReader);

        return await JToken.LoadAsync(jsonTextReader, cancellationToken);
    }

    public async Task<JToken> GetResultsDataAsync(CancellationToken cancellationToken)
    {
        var currentDate = DateTime.Now.ToString("yyyy-MM-dd");
        var fullUrl = _settings.ResultsUrl + currentDate;

        var response = await _httpClient.GetAsync(fullUrl, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        await using var decompressedStream = new GZipStream(responseStream, CompressionMode.Decompress);

        using var jsonReader = new StreamReader(decompressedStream);
        using var jsonTextReader = new JsonTextReader(jsonReader);

        return await JToken.LoadAsync(jsonTextReader, cancellationToken);
    }
}