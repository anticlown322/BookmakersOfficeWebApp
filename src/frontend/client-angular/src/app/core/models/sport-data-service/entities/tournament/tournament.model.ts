import { Match } from "../match/match.model";

export interface Tournament {
  id: string;
  tournamentId: string;
  name: string;
  matches: Match[];
}