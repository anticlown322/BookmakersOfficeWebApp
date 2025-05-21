using Newtonsoft.Json.Linq;

namespace SportDataService.Infrastructure.Services.DataCollection.Abstractions;

public interface IApiDataService
{
    Task<JToken> GetMarketsDataAsync(CancellationToken cancellationToken);
    Task<JToken> GetResultsDataAsync(CancellationToken cancellationToken);
}