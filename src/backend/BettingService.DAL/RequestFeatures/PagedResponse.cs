using SportDataService.Domain.RequestFeatures;

namespace BettingService.DAL.RequestFeatures;

public record PagedResponse<T>(
    T Data,
    MetaData MetaData);