import { ResultStatus } from "./result-status.enum";
import { SubScore } from "./sub-score.model";

export interface MatchEventResult {
  id: string;
  matchEventResultId?: string;
  parentMatchResultId?: string;
  eventName?: string;
  status: ResultStatus;
  team1TotalScore: number;
  team2TotalScore: number;
  subScores?: SubScore[];
}