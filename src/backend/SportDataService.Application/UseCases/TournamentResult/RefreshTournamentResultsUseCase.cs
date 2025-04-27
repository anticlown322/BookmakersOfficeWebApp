using SportDataService.Application.Contracts.Services;
using SportDataService.Application.Contracts.UseCases.TournamentResult;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.TournamentResult;

using Team = Domain.Models.Common.Team;
using TournamentResult = Domain.Models.Results.TournamentResult;
using MatchResult = Domain.Models.Results.MatchResult;

public class RefreshTournamentResultsUseCase(
    IDataCollectionService dataCollectionService,
    ITournamentResultRepository tournamentResultRepository,
    IMatchResultRepository matchResultRepository,
    ITeamRepository teamRepository)
    : IRefreshTournamentResultsUseCase
{
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var apiAnswer = await dataCollectionService.GetTournamentsResultsInfoAsync(cancellationToken);
        await UpdateDatabase(apiAnswer, cancellationToken);
    }

    private async Task UpdateDatabase(List<TournamentResult> apiAnswer, CancellationToken ct)
    {
        foreach (var tournamentResult in apiAnswer)
        {
            ct.ThrowIfCancellationRequested();

            await UpdateTeams(tournamentResult, ct);

            ct.ThrowIfCancellationRequested();

            await UpdateMatches(tournamentResult, ct);

            ct.ThrowIfCancellationRequested();

            var existingTournamentResult =
                await tournamentResultRepository.GetTournamentResultByTournamentResultIdAsync(
                    tournamentResult.TournamentId,
                    ct);
            if (existingTournamentResult != null)
            {
                ct.ThrowIfCancellationRequested();

                await UpdateExistingTournamentResult(existingTournamentResult, tournamentResult, ct);
            }
            else
            {
                ct.ThrowIfCancellationRequested();

                await tournamentResultRepository.CreateAsync(tournamentResult, ct);
            }
        }
    }

    private async Task UpdateTeams(TournamentResult tournament, CancellationToken ct)
    {
        var allTeams = tournament.Matches
            .SelectMany(m => new[] { m.Team1, m.Team2 })
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
    }

    private async Task UpdateMatches(TournamentResult tournament, CancellationToken ct)
    {
        var allMatches = tournament.Matches.ToList();

        foreach (var match in allMatches)
        {
            var team1 = await teamRepository.GetTeamByTeamIdAsync(match.Team1.TeamId, ct);
            var team2 = await teamRepository.GetTeamByTeamIdAsync(match.Team2.TeamId, ct);

            match.Team1.Id = team1?.Id;
            match.Team2
                .Id = team2?.Id;

            var existingMatch = await matchResultRepository.GetMatchResultByMatchResultIdAsync(match.MatchResultId, ct);
            if (existingMatch != null)
            {
                ct.ThrowIfCancellationRequested();

                match.Team1.Id = existingMatch.Team1.Id ?? team1?.Id;
                match.Team2.Id = existingMatch.Team2.Id ?? team2?.Id;

                await UpdateExistingMatch(existingMatch, match, ct);
            }
            else
            {
                ct.ThrowIfCancellationRequested();
                await matchResultRepository.CreateAsync(match, ct);
            }
        }
    }

    private async Task UpdateExistingTournamentResult(
        TournamentResult existing,
        TournamentResult newTournamentResult,
        CancellationToken ct)
    {
        existing.TournamentName = newTournamentResult.TournamentName;

        var matchesToRemove = existing.Matches
            .Where(em => !newTournamentResult.Matches.Any(nm => nm.MatchResultId == em.MatchResultId))
            .ToList();

        foreach (var match in matchesToRemove)
        {
            existing.Matches.Remove(match);

            ct.ThrowIfCancellationRequested();

            await matchResultRepository.DeleteAsync(match.Id, ct);
        }

        ct.ThrowIfCancellationRequested();

        await tournamentResultRepository.UpdateAsync(existing, ct);
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
        MatchResult existing,
        MatchResult newMatch,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        newMatch.Id = existing.Id;

        await matchResultRepository.UpdateAsync(newMatch, ct);
    }
}