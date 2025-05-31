using AutoMapper;
using Microsoft.Extensions.Logging;
using SportDataService.Application.Contracts.Services;
using SportDataService.Application.Contracts.Services.Signaling;
using SportDataService.Application.Contracts.UseCases.Tournament;
using SportDataService.Application.DTO.Prematch;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Application.UseCases.Tournament;

using Tournament = Domain.Models.Prematch.Tournament;
using Team = Domain.Models.Common.Team;
using Match = Domain.Models.Prematch.Match;

public class RefreshTournamentsUseCase(
    IDataCollectionService dataCollectionService,
    ITournamentRepository tournamentRepository,
    IMatchRepository matchRepository,
    ITeamRepository teamRepository,
    ILogger<RefreshTournamentsUseCase> logger,
    IPrematchNotificationService notifier,
    IMapper mapper)
    : IRefreshTournamentsUseCase
{
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Refreshing tournaments...");

        cancellationToken.ThrowIfCancellationRequested();

        var apiAnswer = await dataCollectionService.GetTournamentsInfoAsync(cancellationToken);
        await UpdateDatabase(apiAnswer, cancellationToken);
        await NotifyClients(cancellationToken);

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

        logger.LogInformation("Tournaments successfully updated");
    }

    private async Task UpdateTeams(Tournament tournament, CancellationToken ct)
    {
        var allTeams = tournament.Matches
            .SelectMany(m => new[] { m.Opponent1, m.Opponent2 })
            .ToList();

        foreach (var team in allTeams)
        {
            var existingTeam = await teamRepository.GetTeamByTeamIdAsync(team.TeamId, ct);
            if (existingTeam != null)
            {
                ct.ThrowIfCancellationRequested();

                ct.ThrowIfCancellationRequested();

                team.Id = existingTeam.Id;

                await teamRepository.UpdateAsync(existingTeam, ct);
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
            var existingMatch = await matchRepository.GetMatchByMatchIdAsync(match.MatchId, ct);
            if (existingMatch != null)
            {
                ct.ThrowIfCancellationRequested();

                var team1 = await teamRepository.GetTeamByTeamIdAsync(match.Opponent1.TeamId, ct);

                ct.ThrowIfCancellationRequested();

                var team2 = await teamRepository.GetTeamByTeamIdAsync(match.Opponent2.TeamId, ct);

                match.Id = existingMatch.Id;
                match.MatchId = existingMatch.MatchId;
                match.Opponent1.Id = existingMatch.Opponent1.Id ?? team1?.Id;
                match.Opponent2.Id = existingMatch.Opponent2.Id ?? team2?.Id;

                ct.ThrowIfCancellationRequested();

                await matchRepository.UpdateAsync(match, ct);
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
        ct.ThrowIfCancellationRequested();

        var matchesToRemove = existing.Matches
            .Where(em => newTournament.Matches.All(nm => nm.MatchId != em.MatchId))
            .ToList();

        var updateTime = DateTime.UtcNow;
        var startedMatches = existing.Matches
            .Where(m => m.StartTime.Value.ToUniversalTime() <= updateTime.ToUniversalTime())
            .ToList();

        var allMatchesToRemove = matchesToRemove.Union(startedMatches).Distinct().ToList();

        foreach (var match in allMatchesToRemove)
        {
            ct.ThrowIfCancellationRequested();
            await matchRepository.DeleteAsync(match.Id, ct);

            newTournament.Matches.RemoveAll(m => m.MatchId == match.MatchId);
        }

        newTournament.Id = existing.Id;
        newTournament.Matches = newTournament.Matches
            .Where(m => !allMatchesToRemove.Any(x => x.MatchId == m.MatchId))
            .ToList();

        ct.ThrowIfCancellationRequested();

        await tournamentRepository.DeleteAsync(existing.Id, ct);

        ct.ThrowIfCancellationRequested();

        await tournamentRepository.CreateAsync(newTournament, ct);
    }

    private async Task NotifyClients(CancellationToken ct)
    {
        var tournamentParameters = new TournamentParameters();
        var tournamentsWithMetaData =
            await tournamentRepository.FindAllTournamentsAsync(tournamentParameters, ct);

        var tournamentGetDtos = mapper.Map<IEnumerable<TournamentGetDto>>(tournamentsWithMetaData);
        await notifier.NotifyPrematchUpdatedAsync(tournamentGetDtos, tournamentsWithMetaData.MetaData);
    }
}