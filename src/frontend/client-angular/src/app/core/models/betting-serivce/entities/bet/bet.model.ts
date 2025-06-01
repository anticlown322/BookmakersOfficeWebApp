import { BetStatus } from "./bet-status.enum";

export interface Bet {
  id: string;
  username: string;
  matchId: string;
  amount: number;
  lineType: string;
  marketSelection: string;
  odds: number;
  status: BetStatus;
  createdAt: Date;
  updatedAt?: Date;
}
