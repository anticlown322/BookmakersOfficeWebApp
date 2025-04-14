using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using SportDataService.Domain.Models;
using SportDataService.Domain.Models.Lines;
using SportDataService.Domain.Models.Markets;
using SportDataService.Domain.Models.Tournaments;

namespace SportDataService.Infrastructure.Configs;

public static class MongoDbMappingConfig
{
    public static void ConfigureMappings()
    {
        var conventionPack = new ConventionPack
        {
            new CamelCaseElementNameConvention(),
            new IgnoreExtraElementsConvention(true),
            new EnumRepresentationConvention(BsonType.String),
            new ImmutableTypeClassMapConvention(),
        };
        ConventionRegistry.Register("CustomConventions", conventionPack, _ => true);

        ConfigureTeamMapping();
        ConfigureMatchMapping();
        ConfigureTournamentMapping();
    }

    private static void ConfigureTeamMapping()
    {
        BsonClassMap.RegisterClassMap<Team>(cm =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);

            cm.MapIdProperty(t => t.Id)
                .SetIdGenerator(StringObjectIdGenerator.Instance)
                .SetSerializer(new StringSerializer(BsonType.ObjectId));
            cm.GetMemberMap(c => c.Name).SetElementName("name");
        });
    }

    private static void ConfigureMatchMapping()
    {
        BsonClassMap.RegisterClassMap<Match>(cm =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);

            cm.MapIdProperty(m => m.Id)
                .SetIdGenerator(StringObjectIdGenerator.Instance)
                .SetSerializer(new StringSerializer(BsonType.ObjectId));
            cm.GetMemberMap(c => c.MatchId).SetElementName("matchId");
            cm.GetMemberMap(c => c.TournamentId).SetElementName("tournamentId");
            cm.GetMemberMap(c => c.StartTime).SetElementName("startTime");

            cm.GetMemberMap(c => c.Opponent1).SetElementName("opponent1");
            cm.GetMemberMap(c => c.Opponent2).SetElementName("opponent2");

            cm.GetMemberMap(c => c.MainLine).SetElementName("mainLine");
            cm.GetMemberMap(c => c.KillsLine).SetElementName("killsLine");
            cm.GetMemberMap(c => c.MapsLine).SetElementName("mapsLine");
            cm.GetMemberMap(c => c.SpecialLine).SetElementName("specialLine");
        });

        BsonClassMap.RegisterClassMap<MainLine>(cm =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);
        });

        BsonClassMap.RegisterClassMap<KillsLine>(cm =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);
        });

        BsonClassMap.RegisterClassMap<MapsLine>(cm =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);
        });

        BsonClassMap.RegisterClassMap<SpecialLine>(cm =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);
        });

        BsonClassMap.RegisterClassMap<MarketValue>(cm =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);
        });
    }

    private static void ConfigureTournamentMapping()
    {
        BsonClassMap.RegisterClassMap<Tournament>(cm =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);

            cm.MapIdProperty(t => t.Id)
                .SetIdGenerator(StringObjectIdGenerator.Instance)
                .SetSerializer(new StringSerializer(BsonType.ObjectId));
            cm.GetMemberMap(c => c.TournamentId).SetElementName("tournamentId");
            cm.GetMemberMap(c => c.Name).SetElementName("name");
            cm.GetMemberMap(c => c.Matches).SetElementName("matches");
        });
    }
}