using MongoDB.Bson;
using MongoDB.Driver;

namespace SportDataService.Infrastructure.Configs;

public class MongoDbValidatorsConfig(IMongoDatabase database)
{
    public async Task ConfigureSchemaValidationsAsync()
    {
        var validators = new Dictionary<string, BsonDocument>
        {
            { "teams", ConfigureTeamValidation() },
            { "matches", ConfigureMatchValidation() },
            { "tournaments", ConfigureTournamentValidation() },
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

    private BsonDocument ConfigureTeamValidation()
    {
        var teamsValidator = new BsonDocument
        {
            {
                "$jsonSchema", new BsonDocument
                {
                    { "bsonType", "object" },
                    { "required", new BsonArray { "name", "teamId" } },
                    {
                        "properties", new BsonDocument
                        {
                            { "_id", new BsonDocument { { "bsonType", "objectId" } } },
                            {
                                "teamId", new BsonDocument
                                {
                                    { "bsonType", "string" },
                                    { "description", "must be a string and is required" },
                                    { "minLength", 1 },
                                    { "maxLength", 50 },
                                }
                            },
                            {
                                "name", new BsonDocument
                                {
                                    { "bsonType", "string" },
                                    { "description", "must be a string and is required" },
                                    { "minLength", 2 },
                                    { "maxLength", 100 },
                                }
                            },
                        }
                    },
                    { "additionalProperties", false },
                }
            },
        };

        return teamsValidator;
    }

    private BsonDocument ConfigureMatchValidation()
    {
        return new BsonDocument
        {
            {
                "$jsonSchema", new BsonDocument
                {
                    { "bsonType", "object" },
                    { "required", new BsonArray { "matchId", "tournamentId", "startTime", "opponent1", "opponent2" } },
                    {
                        "properties", new BsonDocument
                        {
                            { "_id", new BsonDocument { { "bsonType", "objectId" } } },
                            {
                                "matchId", new BsonDocument
                                {
                                    { "bsonType", "string" },
                                    { "minLength", 1 },
                                    { "maxLength", 50 },
                                }
                            },
                            {
                                "tournamentId", new BsonDocument
                                {
                                    { "bsonType", "string" },
                                    { "minLength", 1 },
                                }
                            },
                            {
                                "startTime", new BsonDocument
                                {
                                    { "bsonType", "date" },
                                    { "description", "Match start time in UTC + 3 (Moscow)" },
                                }
                            },
                            {
                                "opponent1", new BsonDocument
                                {
                                    { "bsonType", "object" },
                                    { "required", new BsonArray { "teamId", "name" } },
                                    { "properties", ConfigureTeamValidation() },
                                }
                            },
                            {
                                "opponent2", new BsonDocument
                                {
                                    { "bsonType", "object" },
                                    { "required", new BsonArray { "teamId", "name" } },
                                    { "properties", ConfigureTeamValidation() },
                                }
                            },
                            { "mainLine", MainLineValidationSchema() },
                            { "killsLine", KillsLineValidationSchema() },
                            { "mapsLine", MapsLineValidationSchema() },
                            { "specialLine", SpecialLineValidationSchema() },
                        }
                    },
                    { "additionalProperties", false },
                }
            },
        };
    }

    private BsonDocument MainLineValidationSchema()
    {
        return new BsonDocument
        {
            { "bsonType", new BsonArray { "object", "null" } },
            {
                "properties", new BsonDocument
                {
                    { "opponent1Win", MarketValueValidationSchema() },
                    { "opponent2Win", MarketValueValidationSchema() },
                    { "draw", MarketValueValidationSchema() },
                    { "opponent1WinOrDraw", MarketValueValidationSchema() },
                    { "opponent2WinOrDraw", MarketValueValidationSchema() },
                }
            },
            { "additionalProperties", false },
        };
    }

    private BsonDocument KillsLineValidationSchema()
    {
        return new BsonDocument
        {
            { "bsonType", new BsonArray { "object", "null" } },
            {
                "properties", new BsonDocument
                {
                    { "opponent1KillsMain", MarketValueValidationSchema() },
                    { "opponent2KillsMain", MarketValueValidationSchema() },
                    { "totalKillsUnder", MarketValueValidationSchema() },
                    { "totalKillsOver", MarketValueValidationSchema() },
                    { "opponent1KillsHandicap", MarketValueValidationSchema() },
                    { "opponent2KillsHandicap", MarketValueValidationSchema() },
                }
            },
            { "additionalProperties", false },
        };
    }

    private BsonDocument MapsLineValidationSchema()
    {
        return new BsonDocument
        {
            { "bsonType", new BsonArray { "object", "null" } },
            {
                "properties", new BsonDocument
                {
                    { "map1HandicapOpponent1", MarketValueValidationSchema() },
                    { "map1HandicapOpponent2", MarketValueValidationSchema() },
                    { "map2HandicapOpponent1", MarketValueValidationSchema() },
                    { "map2HandicapOpponent2", MarketValueValidationSchema() },
                }
            },
            { "additionalProperties", false },
        };
    }

    private BsonDocument SpecialLineValidationSchema()
    {
        return new BsonDocument
        {
            { "bsonType", new BsonArray { "object", "null" } },
            {
                "properties", new BsonDocument
                {
                    { "eitherOpponent1OrOpponent2", MarketValueValidationSchema() },
                }
            },
            { "additionalProperties", false },
        };
    }

    private BsonDocument MarketValueValidationSchema()
    {
        return new BsonDocument
        {
            { "bsonType", new BsonArray { "object", "null" } },
            {
                "properties", new BsonDocument
                {
                    { "value", new BsonDocument { { "bsonType", "string" } } },
                    { "updatedAt", new BsonDocument { { "bsonType", new BsonArray { "date", "null" } } } },
                    { "isActive", new BsonDocument { { "bsonType", "bool" } } },
                }
            },
            { "additionalProperties", false },
        };
    }

    private BsonDocument ConfigureTournamentValidation()
    {
        return new BsonDocument
        {
            {
                "$jsonSchema", new BsonDocument
                {
                    { "bsonType", "object" },
                    { "required", new BsonArray { "tournamentId", "name" } },
                    {
                        "properties", new BsonDocument
                        {
                            { "_id", new BsonDocument { { "bsonType", "objectId" } } },
                            {
                                "tournamentId", new BsonDocument
                                {
                                    { "bsonType", "string" },
                                    { "minLength", 1 },
                                    { "maxLength", 50 },
                                }
                            },
                            {
                                "name", new BsonDocument
                                {
                                    { "bsonType", "string" },
                                    { "minLength", 2 },
                                    { "maxLength", 100 },
                                }
                            },
                            {
                                "matches", new BsonDocument
                                {
                                    { "bsonType", "array" },
                                    {
                                        "items", ConfigureMatchValidation()["$jsonSchema"]
                                    },
                                }
                            },
                        }
                    },
                    { "additionalProperties", false },
                }
            },
        };
    }
}