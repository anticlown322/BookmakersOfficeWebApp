import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { CurrentBalanceResponse } from '../../models/user-service/responses/balance/current-balance.response';
import { DepositRequest } from '../../models/user-service/requests/balance/deposit.request';
import { WithdrawRequest } from '../../models/user-service/requests/balance/withdraw.request';
import { TransactionParameters } from '../../models/user-service/requests/balance/transaction-parameters.request';
import { TransactionHistoryResponse } from '../../models/user-service/responses/balance/transaction-history.response';

@Injectable({ providedIn: 'root' })
export class BalanceService {
    private readonly baseUrl = `${environment.apiUrl}/users`;

    constructor(private http: HttpClient) {}

    getCurrentBalance(username: string): Observable<CurrentBalanceResponse> {
        return this.http.get<CurrentBalanceResponse>(
            `${this.baseUrl}/${username}/balance/current`
        );
    }

    deposit(username: string, dto: DepositRequest): Observable<void> {
        return this.http.post<void>(
            `${this.baseUrl}/${username}/balance/deposit`,
            dto
        );
    }

    withdraw(username: string, dto: WithdrawRequest): Observable<void> {
        return this.http.post<void>(
            `${this.baseUrl}/${username}/balance/withdraw`,
            dto
        );
    }

    getTransactionHistory(
        username: string,
        params: TransactionParameters
    ): Observable<TransactionHistoryResponse> {
        let httpParams = new HttpParams();

        if (params.pageNumber) {
            httpParams = httpParams.set(
                'pageNumber',
                params.pageNumber.toString()
            );
        }

        if (params.pageSize) {
            httpParams = httpParams.set('pageSize', params.pageSize.toString());
        }

        return this.http.get<TransactionHistoryResponse>(
            `${this.baseUrl}/${username}/balance/history`,
            { params: httpParams }
        );
    }

    createTransactionParams(
        pageNumber: number = 1,
        pageSize: number = 10
    ): TransactionParameters {
        return {
            pageNumber,
            pageSize,
        };
    }
}
