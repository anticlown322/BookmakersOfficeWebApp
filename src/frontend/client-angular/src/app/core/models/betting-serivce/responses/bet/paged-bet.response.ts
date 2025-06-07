import { PagedList } from "../../../shared/interfaces/paged-list";
import { Bet } from "../../entities/bet/bet.model";

export interface PagedBetResponse extends PagedList<Bet> {}