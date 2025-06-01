import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { TournamentResultParameters } from '../../models/sport-data-service/requests/tournament-result/tournament-result-parameters.request';
import { PagedTournamentResultResponse } from '../../models/sport-data-service/responses/tournament-result/paged-tournament-result.response';
import { TournamentResult } from '../../models/sport-data-service/entities/tournament-result/tournament-result.model';
import { MetaData } from '../../models/shared/interfaces/meta-data';
import { createPagedRequest } from '../shared/create-paged-request.function';

@Injectable({
    providedIn: 'root',
})
export class TournamentResultService {
    private readonly baseUrl = `${environment.apiUrl}/tournament-results`;

    constructor(private http: HttpClient) {}

    refreshTournamentResults(): Observable<void> {
        return this.http.post<void>(this.baseUrl, {});
    }

    getTournamentResults(parameters?: TournamentResultParameters): Observable<PagedTournamentResultResponse> {
        return createPagedRequest<TournamentResult, TournamentResultParameters>(
            this.http,
            this.baseUrl,
            '',
            parameters
        );
    }

    getTournamentResultById(id: string): Observable<TournamentResult> {
        return this.http.get<TournamentResult>(`${this.baseUrl}/by-id/${id}`);
    }

    getTournamentResultByTournamentResultId(
        tournamentResultId: string
    ): Observable<TournamentResult> {
        return this.http.get<TournamentResult>(
            `${this.baseUrl}/by-result-id/${tournamentResultId}`
        );
    }
}
