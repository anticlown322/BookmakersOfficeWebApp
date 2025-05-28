import { PagedList } from "../../../shared/interfaces/paged-list";
import { TournamentResult } from "../../entities/tournament-result/tournament-result.model";

export interface PagedTournamentResultResponse extends PagedList<TournamentResult> {}