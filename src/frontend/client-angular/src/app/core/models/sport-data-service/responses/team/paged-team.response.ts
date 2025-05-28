import { PagedList } from "../../shared/interfaces/paged-list";
import { Team } from "../entities/team.model";

export interface PagedTeamResponse extends PagedList<Team> {}