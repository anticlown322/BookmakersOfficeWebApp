using MongoDB.Driver;
using SportDataService.Domain.Models.Tournaments;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Domain.RequestFeatures.Params;
using UserService.Domain.RequestFeatures;

namespace SportDataService.Infrastructure.Repository;

public sealed class TeamRepository : MongoRepositoryBase<Team>, ITeamRepository
{
    public TeamRepository(IMongoDatabase database)
        : base(database, "teams")
    {
    }

    public async Task<PagedList<Team>> FindAllTeamsAsync(TeamParameters teamParameters, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var teams = await FindAllAsync(cancellationToken);

        var orderedTeams = teams.OrderBy(p => p.Name);

        var pagedTeams = orderedTeams
            .Skip((teamParameters.PageNumber - 1) * teamParameters.PageSize)
            .Take(teamParameters.PageSize)
            .ToList();

        var totalCount = orderedTeams.Count();

        return new PagedList<Team>(
            pagedTeams,
            totalCount,
            teamParameters.PageNumber,
            teamParameters.PageSize);
    }

    public async Task<Team?> GetTeamByTeamIdAsync(string teamId, CancellationToken ct)
    {
        var filter = Builders<Team>.Filter.Eq(t => t.TeamId, teamId);
        return await Collection.Find(filter).FirstOrDefaultAsync(ct);
    }
}