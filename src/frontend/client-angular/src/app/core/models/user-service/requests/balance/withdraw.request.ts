export interface WithdrawRequest {
    amount: number;
    confirmationCode: string;
    comment?: string;
}