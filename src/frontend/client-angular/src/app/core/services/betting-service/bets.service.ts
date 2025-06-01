import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { PlaceBetRequest } from '../../models/betting-serivce/requests/bet/place-bet.request';
import { BetParameters } from '../../models/betting-serivce/requests/bet/bet-parameters.request';
import { PagedBetResponse } from '../../models/betting-serivce/responses/bet/paged-bet.response';
import { Bet } from '../../models/betting-serivce/entities/bet/bet.model';
import { MetaData } from '../../models/shared/interfaces/meta-data';
import { createPagedRequest } from '../shared/create-paged-request.function';

@Injectable({
    providedIn: 'root',
})
export class BetsService {
    private readonly baseUrl = `${environment.apiUrl}/bets`;

    constructor(private http: HttpClient) {}

    placeBet(betData: PlaceBetRequest): Observable<void> {
        return this.http.post<void>(this.baseUrl, betData);
    }

    updatePendingBets(): Observable<void> {
        return this.http.post<void>(`${this.baseUrl}/pending`, {});
    }

    updateActiveBets(): Observable<void> {
        return this.http.post<void>(`${this.baseUrl}/active`, {});
    }
    
    getAllBets(parameters?: BetParameters): Observable<PagedBetResponse> {
        return createPagedRequest<Bet, BetParameters>(
            this.http,
            this.baseUrl,
            '',
            parameters
        );
    }

    getUserBets(parameters?: BetParameters): Observable<PagedBetResponse> {
        return createPagedRequest<Bet, BetParameters>(
            this.http,
            this.baseUrl,
            'my',
            parameters
        );
    }

    getBetById(betId: string): Observable<Bet> {
        return this.http.get<Bet>(`${this.baseUrl}/${betId}`);
    }
}
