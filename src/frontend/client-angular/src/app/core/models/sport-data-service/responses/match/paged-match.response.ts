import { PagedList } from "../../../shared/interfaces/paged-list";
import { Match } from "../../entities/match/match.model";

export interface PagedMatchResponse extends PagedList<Match> {}