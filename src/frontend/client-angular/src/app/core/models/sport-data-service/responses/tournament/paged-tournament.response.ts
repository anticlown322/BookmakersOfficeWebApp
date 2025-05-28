import { PagedList } from "../../../shared/interfaces/paged-list";
import { Tournament } from "../../entities/tournament/tournament.model";

export interface PagedTournamentResponse extends PagedList<Tournament> {}