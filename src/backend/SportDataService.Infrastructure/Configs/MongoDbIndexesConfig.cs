using MongoDB.Driver;
using SportDataService.Domain.Models;
using SportDataService.Domain.Models.Tournaments;

namespace SportDataService.Infrastructure.Configs;

public class MongoDbIndexesConfig(IMongoDatabase database)
{
    public async Task CreateIndexesAsync()
    {
        await CreateTeamIndexes();
        await CreateMatchIndexes();
        await CreateTournamentIndexes();
    }

    private async Task CreateTeamIndexes()
    {
        var collection = database.GetCollection<Team>("teams");
        var teamIdIndexModel = new CreateIndexModel<Team>(
            Builders<Team>.IndexKeys.Ascending(x => x.TeamId),
            new CreateIndexOptions { Unique = true });

        var nameIndexModel = new CreateIndexModel<Team>(
            Builders<Team>.IndexKeys.Text(x => x.Name));

        await collection.Indexes.CreateManyAsync([
            teamIdIndexModel,
            nameIndexModel
        ]);
    }

    private async Task CreateMatchIndexes()
    {
        var collection = database.GetCollection<Match>("matches");

        var indexes = new List<CreateIndexModel<Match>>
        {
            new (Builders<Match>.IndexKeys.Ascending(m => m.MatchId), new CreateIndexOptions { Unique = true }),
            new (Builders<Match>.IndexKeys.Ascending(m => m.TournamentId)),
            new (Builders<Match>.IndexKeys.Ascending(m => m.StartTime)),
            new (Builders<Match>.IndexKeys.Combine(
                Builders<Match>.IndexKeys.Ascending(m => m.Opponent1.Id),
                Builders<Match>.IndexKeys.Ascending(m => m.Opponent2.Id))),
        };

        await collection.Indexes.CreateManyAsync(indexes);
    }

    private async Task CreateTournamentIndexes()
    {
        var collection = database.GetCollection<Tournament>("tournaments");

        var indexes = new List<CreateIndexModel<Tournament>>
        {
            new (Builders<Tournament>.IndexKeys.Ascending(t => t.TournamentId), new CreateIndexOptions { Unique = true }),
            new (Builders<Tournament>.IndexKeys.Ascending(t => t.Name)),
            new (Builders<Tournament>.IndexKeys.Ascending("matches.matchId")),
        };

        await collection.Indexes.CreateManyAsync(indexes);
    }
}