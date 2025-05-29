import { PagedList } from "../../../shared/interfaces/paged-list";
import { Team } from "../../entities/team/team.model";

export interface PagedTeamResponse extends PagedList<Team> {}