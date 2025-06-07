import { PagedList } from "../../../shared/interfaces/paged-list";
import { Payout } from "../../entities/payout/payout.model";

export interface PagedPayoutResponse extends PagedList<Payout> {}