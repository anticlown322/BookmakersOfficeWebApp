export enum OperationType {
    DEPOSIT = 'Deposit',
    WITHDRAW = 'Withdraw',
}

export enum TransactionStatus {
    PENDING = 'Pending',
    COMPLETED = 'Completed',
    FAILED = 'Failed',
}

export interface Transaction {
    transactionId: string;
    amount: number;
    operationType: OperationType;
    createdAt: Date;
    comment?: string;
    status: TransactionStatus;
}