import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { RequestPayout } from '../../models/betting-serivce/requests/payout/request-payout.request';
import { PayoutParameters } from '../../models/betting-serivce/requests/payout/payout-parameters.request';
import { PagedPayoutResponse } from '../../models/betting-serivce/responses/payout/paged-payout.response';
import { Payout } from '../../models/betting-serivce/entities/payout/payout.model';

@Injectable({
    providedIn: 'root',
})
export class PayoutsService {
    private readonly baseUrl = `${environment.apiUrl}/payouts`;

    constructor(private http: HttpClient) {}

    requestPayout(payoutData: RequestPayout): Observable<void> {
        return this.http.post<void>(this.baseUrl, payoutData);
    }

    processPayouts(): Observable<void> {
        return this.http.post<void>(`${this.baseUrl}/not-completed`, {});
    }

    getAllPayouts(
        parameters?: PayoutParameters
    ): Observable<PagedPayoutResponse> {
        const params = this.createParams(parameters);
        return this.http.get<PagedPayoutResponse>(this.baseUrl, { params });
    }

    getUserPayouts(
        parameters?: PayoutParameters
    ): Observable<PagedPayoutResponse> {
        const params = this.createParams(parameters);
        return this.http.get<PagedPayoutResponse>(`${this.baseUrl}/my`, {
            params,
        });
    }

    getPayoutById(payoutId: string): Observable<Payout> {
        return this.http.get<Payout>(`${this.baseUrl}/${payoutId}`);
    }

    getPayoutByBetId(betId: string): Observable<Payout> {
        return this.http.get<Payout>(`${this.baseUrl}/by-bet-id/${betId}`);
    }

    private createParams(parameters?: PayoutParameters): HttpParams {
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

        return params;
    }
}
