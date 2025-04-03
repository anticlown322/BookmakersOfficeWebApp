using MongoDB.Bson;
using MongoDB.Driver;

namespace SportDataService.Infrastructure.Configs;

public class MongoDbValidatorsConfig(IMongoDatabase database)
{
    public async Task ConfigureSchemaValidationsAsync()
    {
        var matchesValidator = ConfigureMatchesValidation();
        var eventsValidator = ConfigureEventsValidation();
        var oddsValidator = ConfigureOddsValidation();
        var teamsValidator = ConfigureTeamsValidation();
        var leaguesValidator = ConfigureLeaguesValidation();
        var playersValidator = ConfigurePlayersValidation();

        var validators = new Dictionary<string, BsonDocument>
        {
            { "matches", matchesValidator },
            { "events", eventsValidator },
            { "leagues", leaguesValidator },
            { "odds", oddsValidator },
            { "teams", teamsValidator },
            { "players", playersValidator },
        };

        foreach (var (collectionName, validator) in validators)
        {
            var collectionExists = (await database.ListCollectionNamesAsync())
                .ToList()
                .Contains(collectionName);

            if (collectionExists)
            {
                await database.RunCommandAsync<BsonDocument>(new BsonDocument
                {
                    { "collMod", collectionName },
                    { "validator", validator },
                    { "validationLevel", "strict" },
                });
            }
            else
            {
                var options = new CreateCollectionOptions();
                await database.CreateCollectionAsync(collectionName, options);

                await database.RunCommandAsync<BsonDocument>(new BsonDocument
                {
                    { "collMod", collectionName },
                    { "validator", validator },
                    { "validationLevel", "strict" },
                });
            }
        }
    }

    private BsonDocument ConfigureMatchesValidation()
    {
        var matchesValidator = new BsonDocument
        {
            {
                "$jsonSchema", new BsonDocument
                {
                    { "bsonType", "object" },
                    {
                        "required",
                        new BsonArray { "leagueId", "homeTeamId", "awayTeamId", "startTime", "status" }
                    },
                    {
                        "properties", new BsonDocument
                        {
                            { "status", new BsonDocument { { "bsonType", "string" } } },
                            { "startTime", new BsonDocument { { "bsonType", "date" } } },
                            { "currentScore.home", new BsonDocument { { "bsonType", "int" }, { "minimum", 0 } } },
                            { "currentScore.away", new BsonDocument { { "bsonType", "int" }, { "minimum", 0 } } },
                            { "eventIds", new BsonDocument { { "bsonType", "array" } } },
                        }
                    },
                }
            },
        };

        return matchesValidator;
    }

    private BsonDocument ConfigureEventsValidation()
    {
        var eventsValidator = new BsonDocument
        {
            {
                "$jsonSchema", new BsonDocument
                {
                    { "bsonType", "object" },
                    { "required", new BsonArray { "matchId", "type", "minute", "teamId" } },
                    {
                        "properties", new BsonDocument
                        {
                            { "type", new BsonDocument { { "bsonType", "string" } } },
                            { "minute", new BsonDocument { { "minimum", 0 }, { "maximum", 120 } } },
                            { "teamId", new BsonDocument { { "bsonType", "string" } } },
                            { "playerId", new BsonDocument { { "bsonType", "string" } } },
                        }
                    },
                }
            },
        };

        return eventsValidator;
    }

    private BsonDocument ConfigureOddsValidation()
    {
        var oddsValidator = new BsonDocument
        {
            {
                "$jsonSchema", new BsonDocument
                {
                    { "bsonType", "object" },
                    { "required", new BsonArray { "matchId", "marketType", "odds", "timestamp" } },
                    {
                        "properties", new BsonDocument
                        {
                            {
                                "marketType",
                                new BsonDocument { { "enum", new BsonArray { "1x2", "handicap", "total" } } }
                            },
                            { "odds.home", new BsonDocument { { "bsonType", "decimal" } } },
                            { "odds.draw", new BsonDocument { { "bsonType", "decimal" } } },
                            { "odds.away", new BsonDocument { { "bsonType", "decimal" } } },
                            { "timestamp", new BsonDocument { { "bsonType", "date" } } },
                        }
                    },
                }
            },
        };

        return oddsValidator;
    }

    private BsonDocument ConfigureLeaguesValidation()
    {
        var leaguesValidator = new BsonDocument
        {
            {
                "$jsonSchema", new BsonDocument
                {
                    { "bsonType", "object" },
                    { "required", new BsonArray { "name", "sportType" } },
                    {
                        "properties", new BsonDocument
                        {
                            {
                                "sportType",
                                new BsonDocument { { "enum", new BsonArray { "football", "basketball", "tennis" } } }
                            },
                        }
                    },
                }
            },
        };

        return leaguesValidator;
    }

    private BsonDocument ConfigureTeamsValidation()
    {
        var teamsValidator = new BsonDocument
        {
            {
                "$jsonSchema", new BsonDocument
                {
                    { "bsonType", "object" },
                    { "required", new BsonArray { "name", "sportType" } },
                    {
                        "properties", new BsonDocument
                        {
                            {
                                "sportType",
                                new BsonDocument { { "enum", new BsonArray { "football", "basketball", "tennis" } } }
                            },
                            { "country", new BsonDocument { { "bsonType", "string" } } },
                            { "players", new BsonDocument { { "bsonType", "array" } } },
                        }
                    },
                }
            },
        };

        return teamsValidator;
    }

    private BsonDocument ConfigurePlayersValidation()
    {
        var playersValidator = new BsonDocument
        {
            {
                "$jsonSchema", new BsonDocument
                {
                    { "bsonType", "object" },
                    { "required", new BsonArray { "name", "birthDate", "nationality" } },
                    {
                        "properties", new BsonDocument
                        {
                            { "name", new BsonDocument { { "bsonType", "string" }, { "minLength", 1 } } },
                            { "teamId", new BsonDocument { { "bsonType", "string" } } },
                            { "position", new BsonDocument { { "bsonType", "string" } } },
                            {
                                "number",
                                new BsonDocument
                                {
                                    { "bsonType", "int" },
                                    { "minimum", 1 },
                                    { "maximum", 99 },
                                }
                            },
                            { "birthDate", new BsonDocument { { "bsonType", "date" } } },
                            { "nationality", new BsonDocument { { "bsonType", "string" }, { "minLength", 2 } } },
                        }
                    },
                }
            },
        };

        return playersValidator;
    }
}