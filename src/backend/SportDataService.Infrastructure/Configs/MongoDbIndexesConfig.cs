using MongoDB.Driver;
using SportDataService.Domain.Models;

namespace SportDataService.Infrastructure.Configs;

public class MongoDbIndexesConfig(IMongoDatabase database)
{
    public async Task CreateIndexesAsync()
    {
        await CreateMatchIndexes();
        await CreateEventIndexes();
        await CreateOddsIndexes();
        await CreateLeagueIndexes();
        await CreateTeamIndexes();
    }

    private async Task CreateMatchIndexes()
    {
        var matchesCollection = database.GetCollection<Match>("matches");
        var matchesIndexKeys = Builders<Match>.IndexKeys
            .Ascending(m => m.LeagueId)
            .Ascending(m => m.StartTime);

        await matchesCollection.Indexes.CreateOneAsync(
            new CreateIndexModel<Match>(matchesIndexKeys));
    }

    private async Task CreateOddsIndexes()
    {
        var oddsCollection = database.GetCollection<Odds>("odds");
        var oddsIndexKeys = Builders<Odds>.IndexKeys
            .Ascending(o => o.MatchId)
            .Ascending(o => o.MarketType);

        await oddsCollection.Indexes.CreateOneAsync(
            new CreateIndexModel<Odds>(oddsIndexKeys, new CreateIndexOptions { Unique = true }));

        var ttlIndexKeys = Builders<Odds>.IndexKeys
            .Ascending(o => o.Timestamp);

        await oddsCollection.Indexes.CreateOneAsync(
            new CreateIndexModel<Odds>(ttlIndexKeys, new CreateIndexOptions
            {
                ExpireAfter = TimeSpan.FromDays(7),
            }));
    }

    private async Task CreateEventIndexes()
    {
        var eventsCollection = database.GetCollection<Event>("events");
        var eventsIndexKeys = Builders<Event>.IndexKeys
            .Ascending(e => e.MatchId)
            .Ascending(e => e.Minute);

        await eventsCollection.Indexes.CreateOneAsync(
            new CreateIndexModel<Event>(eventsIndexKeys));
    }

    private async Task CreateLeagueIndexes()
    {
        var leaguesCollection = database.GetCollection<League>("leagues");
        var leaguesIndexKeys = Builders<League>.IndexKeys
            .Ascending(l => l.SportType)
            .Ascending(l => l.Country);

        await leaguesCollection.Indexes.CreateOneAsync(
            new CreateIndexModel<League>(leaguesIndexKeys));
    }

    private async Task CreateTeamIndexes()
    {
        var teamsCollection = database.GetCollection<Team>("teams");
        var teamsIndexKeys = Builders<Team>.IndexKeys
            .Ascending(t => t.SportType)
            .Ascending(t => t.Country);

        await teamsCollection.Indexes.CreateOneAsync(
            new CreateIndexModel<Team>(teamsIndexKeys));
    }
}