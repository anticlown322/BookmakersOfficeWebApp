import { PagedList } from "../../../shared/interfaces/paged-list";
import { MatchResult } from "../../entities/match-result/match-result.model";

export interface PagedMatchResultResponse extends PagedList<MatchResult> {}