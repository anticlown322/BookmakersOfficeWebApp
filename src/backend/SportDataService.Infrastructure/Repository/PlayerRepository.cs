using MongoDB.Driver;
using SportDataService.Domain.Models;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Domain.RequestFeatures.Params;
using UserService.Domain.RequestFeatures;

namespace SportDataService.Infrastructure.Repository;

public sealed class PlayerRepository : MongoRepositoryBase<Player>, IPlayerRepository
{
    public PlayerRepository(IMongoDatabase database)
        : base(database, "players")
    {
    }

    public async Task<PagedList<Player>> FindAllPlayersAsync(PlayerParameters playerParameters, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var players = await FindAllAsync(cancellationToken);

        var orderedPlayers = players.OrderBy(p => p.Name);

        var pagedPlayers = orderedPlayers
            .Skip((playerParameters.PageNumber - 1) * playerParameters.PageSize)
            .Take(playerParameters.PageSize)
            .ToList();

        var totalCount = orderedPlayers.Count();

        return new PagedList<Player>(
            pagedPlayers,
            totalCount,
            playerParameters.PageNumber,
            playerParameters.PageSize);
    }
}