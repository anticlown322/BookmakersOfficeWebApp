using MongoDB.Driver;
using SportDataService.Application.Contracts.Services;
using SportDataService.Application.Contracts.UseCases.Tournament;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.Tournament;
using Tournament = Domain.Models.Tournaments.Tournament;
using Team = Domain.Models.Tournaments.Team;
using Match = Domain.Models.Tournaments.Match;

public class ForceTournamentRefresh(
    IDataCollectionService dataCollectionService,
    ITournamentRepository tournamentRepository,
    IMatchRepository matchRepository,
    ITeamRepository teamRepository)
    : IForceTournamentRefresh
{
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var apiAnswer = await dataCollectionService.GetTournamentsInfoAsync(cancellationToken);
        await UpdateDatabase(apiAnswer, cancellationToken);
    }

    private async Task UpdateDatabase(List<Tournament> apiAnswer, CancellationToken ct)
    {
        foreach (var tournament in apiAnswer)
        {
            ct.ThrowIfCancellationRequested();

            await UpdateTeams(tournament, ct);
            await UpdateMatches(tournament, ct);

            ct.ThrowIfCancellationRequested();

            var existingTournament = await tournamentRepository.GetByTournamentIdAsync(tournament.TournamentId, ct);
            if (existingTournament != null)
            {
                ct.ThrowIfCancellationRequested();

                await UpdateExistingTournament(existingTournament, tournament, ct);
            }
            else
            {
                ct.ThrowIfCancellationRequested();

                await tournamentRepository.CreateAsync(tournament, ct);
            }
        }
    }

    private async Task UpdateTeams(Tournament tournament, CancellationToken ct)
    {
        var allTeams = tournament.Matches
            .SelectMany(m => new[] { m.Opponent1, m.Opponent2 })
            .DistinctBy(t => t.TeamId)
            .ToList();

        foreach (var team in allTeams)
        {
            var existingTeam = await teamRepository.GetByTeamIdAsync(team.TeamId, ct);
            if (existingTeam != null)
            {
                ct.ThrowIfCancellationRequested();

                await UpdateExistingTeam(existingTeam, team, ct);
            }
            else
            {
                ct.ThrowIfCancellationRequested();

                await teamRepository.CreateAsync(team, ct);
            }
        }
    }

    private async Task UpdateMatches(Tournament tournament, CancellationToken ct)
    {
        var allMatches = tournament.Matches.ToList();

        foreach (var match in allMatches)
        {
            var existingMatch = await matchRepository.GetByMatchIdAsync(match.MatchId, ct);
            if (existingMatch != null)
            {
                ct.ThrowIfCancellationRequested();

                await UpdateExistingMatch(existingMatch, match, ct);
            }
            else
            {
                ct.ThrowIfCancellationRequested();

                await matchRepository.CreateAsync(match, ct);
            }
        }
    }

    private async Task UpdateExistingTournament(
        Tournament existing,
        Tournament newTournament,
        CancellationToken ct)
    {
        existing.Name = newTournament.Name;

        var matchesToRemove = existing.Matches
            .Where(em => !newTournament.Matches.Any(nm => nm.MatchId == em.MatchId))
            .ToList();

        foreach (var match in matchesToRemove)
        {
            existing.Matches.Remove(match);

            ct.ThrowIfCancellationRequested();

            await matchRepository.DeleteAsync(match.Id, ct);
        }

        foreach (var newMatch in newTournament.Matches)
        {
            var existingMatch = existing.Matches.FirstOrDefault(m => m.MatchId == newMatch.MatchId);
            if (existingMatch != null)
            {
                ct.ThrowIfCancellationRequested();

                await matchRepository.UpdateAsync(newMatch, ct);
            }
            else
            {
                existing.Matches.Add(newMatch);

                ct.ThrowIfCancellationRequested();

                await matchRepository.CreateAsync(newMatch, ct);
            }
        }

        ct.ThrowIfCancellationRequested();

        await tournamentRepository.UpdateAsync(existing, ct);
    }

    private async Task UpdateExistingTeam(
        Team existing,
        Team newTeam,
        CancellationToken ct)
    {
        if (existing.Name == newTeam.Name)
        {
            return;
        }

        ct.ThrowIfCancellationRequested();

        newTeam.Id = existing.Id;

        await teamRepository.UpdateAsync(newTeam, ct);
    }

    private async Task UpdateExistingMatch(
        Match existing,
        Match newMatch,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        newMatch.Id = existing.Id;

        await matchRepository.UpdateAsync(newMatch, ct);
    }
}