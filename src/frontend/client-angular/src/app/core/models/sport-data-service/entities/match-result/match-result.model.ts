import { Team } from "../team/team.model";
import { MatchEventResult } from "./match-event-result.model";
import { ResultStatus } from "./result-status.enum";
import { SubScore } from "./sub-score.model";

export interface MatchResult {
  id: string;
  matchResultId?: string;
  tournamentId?: string;
  matchName?: string;
  team1: Team;
  team2: Team;
  resultTime?: Date;
  team1TotalScore: number;
  team2TotalScore: number;
  subScores: SubScore[];
  eventResults: MatchEventResult[];
  status: ResultStatus;
}