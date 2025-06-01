export enum BalanceOperationType {
  DEPOSIT = 'Deposit',
  WITHDRAW = 'Withdraw',
}

export interface BalanceTransaction {
  id?: number;                  
  amount: number;
  createdAt: Date;
  operationType: BalanceOperationType; 
  comment?: string;
}