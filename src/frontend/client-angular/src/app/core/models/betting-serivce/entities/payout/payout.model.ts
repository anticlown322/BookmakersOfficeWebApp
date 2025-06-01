import { PayoutStatus } from "./payout-status.enum";

export interface Payout {
  id: string;
  betId: string;
  amount: number;
  status: PayoutStatus;
  processedAt: Date;
  errorReason?: string;
}