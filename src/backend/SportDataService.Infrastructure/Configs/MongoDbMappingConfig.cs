using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using SportDataService.Domain.Models;
using SportDataService.Domain.Models.Common;
using SportDataService.Domain.Models.Prematch;
using SportDataService.Domain.Models.Prematch.Lines;
using SportDataService.Domain.Models.Prematch.Markets;
using SportDataService.Domain.Models.Results;

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

        ConfigureTournamentResultMapping();
        ConfigureMatchResultMapping();
        ConfigureMatchEventResultMapping();
        ConfigureSubScoreMapping();
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

    private static void ConfigureTournamentResultMapping()
    {
        BsonClassMap.RegisterClassMap<TournamentResult>(cm =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);

            cm.MapIdProperty(t => t.Id)
                .SetIdGenerator(StringObjectIdGenerator.Instance)
                .SetSerializer(new StringSerializer(BsonType.ObjectId));

            cm.MapMember(x => x.TournamentId).SetElementName("tournamentId");
            cm.MapMember(x => x.TournamentName).SetElementName("tournamentName");
            cm.MapMember(x => x.Matches).SetElementName("matches");
        });
    }

    private static void ConfigureMatchResultMapping()
    {
        BsonClassMap.RegisterClassMap<MatchResult>(cm =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);

            cm.MapIdProperty(t => t.Id)
                .SetIdGenerator(StringObjectIdGenerator.Instance)
                .SetSerializer(new StringSerializer(BsonType.ObjectId));

            cm.MapMember(x => x.MatchResultId).SetElementName("matchResultId");
            cm.MapMember(x => x.TournamentId).SetElementName("tournamentId");
            cm.MapMember(x => x.MatchName).SetElementName("matchName");
            cm.MapMember(x => x.Team1).SetElementName("team1");
            cm.MapMember(x => x.Team2).SetElementName("team2");
            cm.MapMember(x => x.ResultTime).SetElementName("resultTime");
            cm.MapMember(x => x.Team1TotalScore).SetElementName("team1TotalScore");
            cm.MapMember(x => x.Team2TotalScore).SetElementName("team2TotalScore");
            cm.MapMember(x => x.SubScores).SetElementName("subScores");
            cm.MapMember(x => x.EventResults).SetElementName("eventResults");
            cm.MapMember(x => x.Status).SetElementName("status")
                .SetSerializer(new EnumSerializer<ResultStatus>(BsonType.String));
        });
    }

    private static void ConfigureMatchEventResultMapping()
    {
        BsonClassMap.RegisterClassMap<MatchEventResult>(cm =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);

            cm.MapIdProperty(t => t.Id)
                .SetIdGenerator(StringObjectIdGenerator.Instance)
                .SetSerializer(new StringSerializer(BsonType.ObjectId));

            cm.MapIdMember(x => x.MatchEventResultId).SetElementName("matchEventResultId");
            cm.MapMember(x => x.ParentMatchResultId).SetElementName("parentMatchResultId");
            cm.MapMember(x => x.EventName).SetElementName("eventName");
            cm.MapMember(x => x.Status).SetElementName("status")
                .SetSerializer(new EnumSerializer<ResultStatus>(BsonType.String));
            cm.MapMember(x => x.Team1TotalScore).SetElementName("team1TotalScore");
            cm.MapMember(x => x.Team2TotalScore).SetElementName("team2TotalScore");
            cm.MapMember(x => x.SubScores).SetElementName("subScores");
        });
    }

    private static void ConfigureSubScoreMapping()
    {
        BsonClassMap.RegisterClassMap<SubScore>(cm =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);

            cm.MapMember(x => x.SubscorePosition).SetElementName("subscorePosition");
            cm.MapMember(x => x.Title).SetElementName("title");
            cm.MapMember(x => x.Team1Score).SetElementName("team1Score");
            cm.MapMember(x => x.Team2Score).SetElementName("team2Score");
        });
    }
}