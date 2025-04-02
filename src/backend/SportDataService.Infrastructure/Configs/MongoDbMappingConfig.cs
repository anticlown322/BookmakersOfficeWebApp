using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using SportDataService.Domain.Models;

namespace SportDataService.Infrastructure.Configs;

public static class MongoDbMappingConfig
{
    public static void ConfigureMappings()
    {
        var conventionPack = new ConventionPack
        {
            new CamelCaseElementNameConvention(),
            new IgnoreExtraElementsConvention(true),
        };
        ConventionRegistry.Register("CustomConventions", conventionPack, _ => true);

        ConfigureMatchMapping();
        ConfigureEventMapping();
        ConfigureOddsMapping();
        ConfigureLeaguesMapping();
        ConfigureTeamMapping();
        ConfigurePlayerMapping();
    }

    private static void ConfigureMatchMapping()
    {
        BsonClassMap.RegisterClassMap<Score>(sm =>
        {
            sm.AutoMap();
            sm.MapProperty(s => s.Home).SetElementName("home");
            sm.MapProperty(s => s.Away).SetElementName("away");
        });

        BsonClassMap.RegisterClassMap<Match>(cm =>
        {
            cm.AutoMap();
            cm.MapIdProperty(m => m.Id)
                .SetIdGenerator(StringObjectIdGenerator.Instance)
                .SetSerializer(new StringSerializer(BsonType.ObjectId));
            cm.MapProperty(m => m.LeagueId).SetElementName("leagueId");
            cm.MapProperty(m => m.HomeTeamId).SetElementName("homeTeamId");
            cm.MapProperty(m => m.AwayTeamId).SetElementName("awayTeamId");
            cm.MapProperty(m => m.Status).SetElementName("status");
            cm.MapProperty(m => m.CurrentScore).SetElementName("currentScore");

            cm.MapProperty(m => m.EventIds)
                .SetElementName("eventIds")
                .SetShouldSerializeMethod(obj => ((Match)obj).EventIds?.Any() == true);

            cm.MapProperty(m => m.StartTime)
                .SetElementName("startTime")
                .SetSerializer(new DateTimeSerializer(DateTimeKind.Utc));

            cm.MapProperty(m => m.CreatedAt)
                .SetElementName("createdAt")
                .SetSerializer(new DateTimeSerializer(DateTimeKind.Utc));

            cm.MapProperty(m => m.UpdatedAt)
                .SetElementName("updatedAt")
                .SetSerializer(new DateTimeSerializer(DateTimeKind.Utc));
        });
    }

    private static void ConfigureEventMapping()
    {
        BsonClassMap.RegisterClassMap<Event>(cm =>
        {
            cm.AutoMap();
            cm.MapIdProperty(e => e.Id)
                .SetIdGenerator(StringObjectIdGenerator.Instance)
                .SetSerializer(new StringSerializer(BsonType.ObjectId));
            cm.MapProperty(e => e.MatchId).SetElementName("matchId");
            cm.MapProperty(e => e.Type).SetElementName("type");
            cm.MapProperty(e => e.Minute).SetElementName("minute");
            cm.MapProperty(e => e.TeamId).SetElementName("teamId");
            cm.MapProperty(e => e.PlayerId).SetElementName("playerId").SetIgnoreIfNull(true);
            cm.MapProperty(e => e.AdditionalInfo).SetElementName("additionalInfo").SetIgnoreIfNull(true);
        });
    }

    private static void ConfigureOddsMapping()
    {
        BsonClassMap.RegisterClassMap<Odds>(cm =>
        {
            cm.AutoMap();
            cm.MapIdProperty(o => o.Id)
                .SetIdGenerator(StringObjectIdGenerator.Instance)
                .SetSerializer(new StringSerializer(BsonType.ObjectId));
            cm.MapProperty(o => o.MatchId).SetElementName("matchId");
            cm.MapProperty(o => o.BookmakerId).SetElementName("bookmakerId");
            cm.MapProperty(o => o.MarketType).SetElementName("marketType");
            cm.MapProperty(o => o.Values).SetElementName("odds");
            cm.MapProperty(o => o.Timestamp).SetElementName("timestamp");
            cm.MapProperty(o => o.IsLive).SetElementName("isLive");
        });
    }

    private static void ConfigureLeaguesMapping()
    {
        BsonClassMap.RegisterClassMap<League>(cm =>
        {
            cm.AutoMap();
            cm.MapIdProperty(l => l.Id)
                .SetIdGenerator(StringObjectIdGenerator.Instance)
                .SetSerializer(new StringSerializer(BsonType.ObjectId));
            cm.MapProperty(l => l.Name).SetElementName("name");
            cm.MapProperty(l => l.Country).SetElementName("country");
            cm.MapProperty(l => l.SportType).SetElementName("sportType");
            cm.MapProperty(l => l.Season).SetElementName("season");
            cm.MapProperty(l => l.IsActive).SetElementName("isActive");
        });
    }

    private static void ConfigureTeamMapping()
    {
        BsonClassMap.RegisterClassMap<Team>(cm =>
        {
            cm.AutoMap();
            cm.MapIdProperty(t => t.Id)
                .SetIdGenerator(StringObjectIdGenerator.Instance)
                .SetSerializer(new StringSerializer(BsonType.ObjectId));
            cm.MapProperty(t => t.Name).SetElementName("name");
            cm.MapProperty(t => t.ShortName).SetElementName("shortName");
            cm.MapProperty(t => t.Country).SetElementName("country");
            cm.MapProperty(t => t.SportType).SetElementName("sportType");
            cm.MapProperty(t => t.PlayerIds).SetElementName("players");
        });
    }

    private static void ConfigurePlayerMapping()
    {
        BsonClassMap.RegisterClassMap<Player>(cm =>
        {
            cm.AutoMap();
            cm.MapIdProperty(p => p.Id)
                .SetIdGenerator(StringObjectIdGenerator.Instance)
                .SetSerializer(new StringSerializer(BsonType.ObjectId));
            cm.MapProperty(p => p.Name).SetElementName("name");
            cm.MapProperty(p => p.TeamId).SetElementName("teamId");
            cm.MapProperty(p => p.Position).SetElementName("position");
            cm.MapProperty(p => p.Number).SetElementName("number");
            cm.MapProperty(p => p.BirthDate).SetElementName("birthDate");
            cm.MapProperty(p => p.Nationality).SetElementName("nationality");
        });
    }
}