import { Team } from "../team/team.model";
import { KillsLine } from "./kills-line.model";
import { MainLine } from "./main-line.model";
import { MapsLine } from "./maps-line.model";
import { SpecialLine } from "./special-line.model";

export interface Match {
  id: string;
  matchId: string;
  tournamentId: string;
  opponent1: Team;
  opponent2: Team;
  startTime: Date;
  mainLine?: MainLine;
  killsLine?: KillsLine;
  mapsLine?: MapsLine;
  specialLine?: SpecialLine;
}