using MongoDB.Bson;
using MongoDB.Driver;
using SportDataService.Domain.Models.Results;

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
            { "matchResults", ConfigureMatchResultValidation() },
            { "tournamentResults", ConfigureTournamentResultValidation() },
        };

        foreach (var (collectionName, validator) in validators)
        {
            var collectionExists = (await database.ListCollectionNamesAsync())
                .ToList()
                .Contains(collectionName);

            if (collectionExists)
            {
                await database.RunCommandAsync<BsonDocument>(
                    new BsonDocument
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

                await database.RunCommandAsync<BsonDocument>(
                    new BsonDocument
                    {
                        { "collMod", collectionName },
                        { "validator", validator },
                        { "validationLevel", "strict" },
                    });
            }
        }
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
                                    { "items", ConfigureMatchValidation()["$jsonSchema"] },
                                }
                            },
                        }
                    },
                    { "additionalProperties", false },
                }
            },
        };
    }

    private BsonDocument ConfigureTournamentResultValidation()
    {
        var validator = new BsonDocument
        {
            {
                "$jsonSchema", new BsonDocument
                {
                    { "bsonType", "object" },
                    { "required", new BsonArray { "tournamentId", "matches" } },
                    {
                        "properties", new BsonDocument
                        {
                            { "_id", new BsonDocument { { "bsonType", "objectId" } } },
                            { "tournamentId", new BsonDocument { { "bsonType", "string" } } },
                            { "tournamentName", new BsonDocument { { "bsonType", "string" } } },
                            {
                                "matches", new BsonDocument
                                {
                                    { "bsonType", "array" },
                                    { "items", ConfigureMatchResultValidation()["$jsonSchema"] },
                                }
                            },
                        }
                    },
                }
            },
        };
        return validator;
    }

    private BsonDocument ConfigureMatchResultValidation()
    {
        return new BsonDocument
        {
            {
                "$jsonSchema", new BsonDocument
                {
                    { "bsonType", "object" },
                    { "required", new BsonArray { "matchResultId", "tournamentId", "team1", "team2", "status" } },
                    {
                        "properties", new BsonDocument
                        {
                            { "_id", new BsonDocument { { "bsonType", "objectId" } } },
                            { "matchResultId", new BsonDocument { { "bsonType", "string" } } },
                            { "tournamentId", new BsonDocument { { "bsonType", "string" } } },
                            { "matchName", new BsonDocument { { "bsonType", "string" } } },
                            {
                                "team1", new BsonDocument
                                {
                                    { "bsonType", "object" },
                                    { "required", new BsonArray { "teamId", "name" } },
                                    { "properties", ConfigureTeamValidation() },
                                }
                            },
                            {
                                "team2", new BsonDocument
                                {
                                    { "bsonType", "object" },
                                    { "required", new BsonArray { "teamId", "name" } },
                                    { "properties", ConfigureTeamValidation() },
                                }
                            },
                            { "resultTime", new BsonDocument { { "bsonType", "date" } } },
                            { "team1TotalScore", new BsonDocument { { "bsonType", "int" } } },
                            { "team2TotalScore", new BsonDocument { { "bsonType", "int" } } },
                            {
                                "status",
                                new BsonDocument { { "enum", new BsonArray(Enum.GetNames(typeof(ResultStatus))) } }
                            },
                            {
                                "subScores", new BsonDocument
                                {
                                    { "bsonType", "array" },
                                    { "items", ConfigureSubScoreValidationSchema() },
                                }
                            },
                            {
                                "eventResults", new BsonDocument
                                {
                                    { "bsonType", "array" },
                                    { "items", ConfigureMatchEventResultValidationSchema()["$jsonSchema"] },
                                }
                            },
                        }
                    },
                }
            },
        };
    }

    private BsonDocument ConfigureMatchEventResultValidationSchema()
    {
        return new BsonDocument
        {
            {
                "$jsonSchema", new BsonDocument
                {
                    { "bsonType", "array" },
                    {
                        "items", new BsonDocument
                        {
                            { "bsonType", "object" },
                            {
                                "required",
                                new BsonArray
                                {
                                    "matchEventResultId", "parentMatchResultId", "status", "team1TotalScore",
                                    "team2TotalScore",
                                }
                            },
                            {
                                "properties", new BsonDocument
                                {
                                    { "_id", new BsonDocument { { "bsonType", "objectId" } } },
                                    { "matchEventResultId", new BsonDocument { { "bsonType", "string" } } },
                                    { "parentMatchResultId", new BsonDocument { { "bsonType", "string" } } },
                                    { "eventName", new BsonDocument { { "bsonType", "string" } } },
                                    {
                                        "status",
                                        new BsonDocument
                                        {
                                            {
                                                "enum",
                                                new BsonArray(Enum.GetNames(typeof(ResultStatus)))
                                            },
                                        }
                                    },
                                    { "team1TotalScore", new BsonDocument { { "bsonType", "int" } } },
                                    { "team2TotalScore", new BsonDocument { { "bsonType", "int" } } },
                                    {
                                        "subScores", new BsonDocument
                                        {
                                            { "bsonType", "array" },
                                            { "items", ConfigureSubScoreValidationSchema() },
                                        }
                                    },
                                }
                            },
                        }
                    },
                }
            },
        };
    }

    private BsonDocument ConfigureSubScoreValidationSchema()
    {
        return new BsonDocument
        {
            { "bsonType", "object" },
            { "required", new BsonArray { "subscorePosition", "team1Score", "team2Score" } },
            {
                "properties", new BsonDocument
                {
                    { "subscorePosition", new BsonDocument { { "bsonType", "int" } } },
                    { "title", new BsonDocument { { "bsonType", "string" } } },
                    { "team1Score", new BsonDocument { { "bsonType", "int" } } },
                    { "team2Score", new BsonDocument { { "bsonType", "int" } } },
                }
            },
        };
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
}