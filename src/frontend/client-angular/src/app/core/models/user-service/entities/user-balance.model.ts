import { BalanceTransaction } from "./balance-transaction.model";

export interface UserBalance {
  currentAmount: number;
  lastUpdated: Date;
  transactions?: BalanceTransaction[]; 
}