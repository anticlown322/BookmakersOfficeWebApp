import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { RequestPayout } from '../../models/betting-serivce/requests/payout/request-payout.request';
import { PayoutParameters } from '../../models/betting-serivce/requests/payout/payout-parameters.request';
import { PagedPayoutResponse } from '../../models/betting-serivce/responses/payout/paged-payout.response';
import { Payout } from '../../models/betting-serivce/entities/payout/payout.model';
import { createPagedRequest } from '../shared/create-paged-request.function';

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
        return createPagedRequest<Payout, PayoutParameters>(
            this.http,
            this.baseUrl,
            '',
            parameters
        );
    }

    getUserPayouts(parameters?: PayoutParameters): Observable<PagedPayoutResponse> {
        return createPagedRequest<Payout, PayoutParameters>(
            this.http,
            this.baseUrl,
            'my',
            parameters
        );
    }

    getPayoutById(payoutId: string): Observable<Payout> {
        return this.http.get<Payout>(`${this.baseUrl}/${payoutId}`);
    }

    getPayoutByBetId(betId: string): Observable<Payout> {
        return this.http.get<Payout>(`${this.baseUrl}/by-bet-id/${betId}`);
    }
}
