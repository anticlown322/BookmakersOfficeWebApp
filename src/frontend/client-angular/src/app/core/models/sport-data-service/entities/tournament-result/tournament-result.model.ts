import { MatchResult } from "../match-result/match-result.model";

export interface TournamentResult {
  id: string;
  tournamentId?: string;
  tournamentName?: string;
  matches: MatchResult[];
}