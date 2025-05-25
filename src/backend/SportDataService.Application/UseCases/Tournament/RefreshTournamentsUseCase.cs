using Microsoft.Extensions.Logging;
using SportDataService.Application.Contracts.Services;
using SportDataService.Application.Contracts.UseCases.Tournament;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.Tournament;

using Tournament = Domain.Models.Prematch.Tournament;
using Team = Domain.Models.Common.Team;
using Match = Domain.Models.Prematch.Match;

public class RefreshTournamentsUseCase(
    IDataCollectionService dataCollectionService,
    ITournamentRepository tournamentRepository,
    IMatchRepository matchRepository,
    ITeamRepository teamRepository,
    ILogger<RefreshTournamentsUseCase> logger)
    : IRefreshTournamentsUseCase
{
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Refreshing tournaments...");

        cancellationToken.ThrowIfCancellationRequested();

        var apiAnswer = await dataCollectionService.GetTournamentsInfoAsync(cancellationToken);
        await UpdateDatabase(apiAnswer, cancellationToken);

        logger.LogInformation("Tournaments successfully refreshed");
    }

    private async Task UpdateDatabase(List<Tournament> apiAnswer, CancellationToken ct)
    {
        logger.LogInformation("Updating tournaments...");

        foreach (var tournament in apiAnswer)
        {
            ct.ThrowIfCancellationRequested();

            await UpdateTeams(tournament, ct);

            ct.ThrowIfCancellationRequested();

            await UpdateMatches(tournament, ct);

            ct.ThrowIfCancellationRequested();

            var existingTournament =
                await tournamentRepository.GetTournamentByTournamentIdAsync(tournament.TournamentId, ct);

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

        ct.ThrowIfCancellationRequested();

        logger.LogInformation("Removing started matches...");

        await RemoveStartedMatches(ct);

        logger.LogInformation("Successfully removed started matches");
        logger.LogInformation("Tournaments successfully updated");
    }

    private async Task UpdateTeams(Tournament tournament, CancellationToken ct)
    {
        logger.LogInformation("Updating teams...");

        var allTeams = tournament.Matches
            .SelectMany(m => new[] { m.Opponent1, m.Opponent2 })
            .ToList();

        foreach (var team in allTeams)
        {
            var existingTeam = await teamRepository.GetTeamByTeamIdAsync(team.TeamId, ct);
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

        logger.LogInformation("Teams successfully updated");
    }

    private async Task UpdateMatches(Tournament tournament, CancellationToken ct)
    {
        logger.LogInformation("Updating matches...");

        var allMatches = tournament.Matches.ToList();

        foreach (var match in allMatches)
        {
            var team1 = await teamRepository.GetTeamByTeamIdAsync(match.Opponent1.TeamId, ct);
            var team2 = await teamRepository.GetTeamByTeamIdAsync(match.Opponent2.TeamId, ct);

            match.Opponent1.Id = team1?.Id;
            match.Opponent2.Id = team2?.Id;

            var existingMatch = await matchRepository.GetMatchByMatchIdAsync(match.MatchId, ct);
            if (existingMatch != null)
            {
                ct.ThrowIfCancellationRequested();

                match.Opponent1.Id = existingMatch.Opponent1.Id ?? team1?.Id;
                match.Opponent2.Id = existingMatch.Opponent2.Id ?? team2?.Id;

                await UpdateExistingMatch(existingMatch, match, ct);
            }
            else
            {
                ct.ThrowIfCancellationRequested();
                await matchRepository.CreateAsync(match, ct);
            }
        }

        logger.LogInformation("Matches successfully updated");
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

    private async Task RemoveStartedMatches(CancellationToken ct)
    {
        var currentTime = DateTime.UtcNow.ToUniversalTime();
        var startedMatches = await matchRepository.GetMatchesStartedBeforeAsync(currentTime, ct);

        foreach (var match in startedMatches)
        {
            ct.ThrowIfCancellationRequested();
            await matchRepository.DeleteAsync(match.Id, ct);
        }
    }
}