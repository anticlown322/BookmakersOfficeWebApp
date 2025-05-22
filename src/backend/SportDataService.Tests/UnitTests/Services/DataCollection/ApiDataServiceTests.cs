using System.IO.Compression;
using System.Net;
using System.Text;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json.Linq;
using SportDataService.Domain.Models.Settings;
using SportDataService.Infrastructure.Services.DataCollection.Implementations;

namespace SportDataService.Tests.UnitTests.Services.DataCollection;

public class ApiDataServiceTests
{
    private readonly Mock<HttpMessageHandler> _handlerMock;
    private readonly HttpClient _httpClient;
    private readonly ApiDataService _service;
    private readonly DataCollectionServiceSettings _settings = new()
    {
        MarketsUrl = "https://api.example.com/markets",
        ResultsUrl = "https://api.example.com/results/"
    };

    public ApiDataServiceTests()
    {
        _handlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_handlerMock.Object);
        _service = new ApiDataService(_httpClient, Options.Create(_settings));
    }

    [Fact]
    public async Task GetMarketsDataAsync_ShouldReturnJToken_WhenResponseIsValid()
    {
        // Arrange
        var expectedJson = JToken.Parse("{\"data\": \"test\"}");
        var compressedData = CompressData(expectedJson.ToString());

        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.RequestUri == new Uri(_settings.MarketsUrl)),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new ByteArrayContent(compressedData)
            });

        // Act
        var result = await _service.GetMarketsDataAsync(CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedJson);
    }

    [Fact]
    public async Task GetResultsDataAsync_ShouldAppendCurrentDate_AndReturnJToken()
    {
        // Arrange
        var expectedDate = DateTime.Now.ToString("yyyy-MM-dd");
        var expectedJson = JToken.Parse("{\"results\": \"data\"}");
        var compressedData = CompressData(expectedJson.ToString());

        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => 
                    r.RequestUri == new Uri(_settings.ResultsUrl + expectedDate)),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new ByteArrayContent(compressedData)
            });

        // Act
        var result = await _service.GetResultsDataAsync(CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedJson);
    }

    [Fact]
    public async Task GetMarketsDataAsync_ShouldThrow_WhenResponseNotSuccessful()
    {
        // Arrange
        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound
            });

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => 
            _service.GetMarketsDataAsync(CancellationToken.None));
    }

    [Fact]
    public async Task GetResultsDataAsync_ShouldThrow_WhenResponseNotSuccessful()
    {
        // Arrange
        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            });

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => 
            _service.GetResultsDataAsync(CancellationToken.None));
    }

    [Fact]
    public async Task GetMarketsDataAsync_ShouldHandleGzip_Properly()
    {
        // Arrange
        var testData = "{\"test\": \"value\"}";

        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new ByteArrayContent(CompressData(testData))
            });

        // Act
        var result = await _service.GetMarketsDataAsync(CancellationToken.None);

        // Assert
        result["test"].Value<string>().Should().Be("value");
    }

    private static byte[] CompressData(string data)
    {
        using var memoryStream = new MemoryStream();
        using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
        using (var writer = new StreamWriter(gzipStream))
        {
            writer.Write(data);
        }
        return memoryStream.ToArray();
    }
}