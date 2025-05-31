import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { CurrentBalanceResponse } from '../../models/user-service/responses/balance/current-balance.response';
import { DepositRequest } from '../../models/user-service/requests/balance/deposit.request';
import { WithdrawRequest } from '../../models/user-service/requests/balance/withdraw.request';
import { TransactionParameters } from '../../models/user-service/requests/balance/transaction-parameters.request';
import { TransactionHistoryResponse } from '../../models/user-service/responses/balance/transaction-history.response';
import { MetaData } from '../../models/shared/interfaces/meta-data';
import { Transaction } from '../../models/user-service/entities/transaction.model';

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
        parameters?: TransactionParameters
    ): Observable<TransactionHistoryResponse> {
        let params = new HttpParams();

        if (parameters) {
            if (parameters.pageNumber) {
                params = params.append(
                    'pageNumber',
                    parameters.pageNumber.toString()
                );
            }
            if (parameters.pageSize) {
                params = params.append(
                    'pageSize',
                    parameters.pageSize.toString()
                );
            }
        }

        return this.http
            .get<Transaction[]>(`${this.baseUrl}/${username}/balance/history`, {
                params,
                observe: 'response',
            })
            .pipe(
                map((response) => {
                    const paginationHeader =
                        response.headers.get('X-Pagination') ||
                        response.headers.get('x-pagination');

                    const pagination: MetaData = paginationHeader
                        ? JSON.parse(paginationHeader)
                        : null;

                    return {
                        items: response.body || [],
                        metaData: pagination,
                    };
                })
            );
    }
}
