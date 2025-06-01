import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { MatchResultParameters } from '../../models/sport-data-service/requests/match-result/match-result-parameters.request';
import { PagedMatchResultResponse } from '../../models/sport-data-service/responses/match-result/paged-match-result.response';
import { MatchResult } from '../../models/sport-data-service/entities/match-result/match-result.model';
import { MetaData } from '../../models/shared/interfaces/meta-data';
import { createPagedRequest } from '../shared/create-paged-request.function';

@Injectable({
    providedIn: 'root',
})
export class MatchResultService {
    private readonly baseUrl = `${environment.apiUrl}/match-results`;

    constructor(private http: HttpClient) {}

    getMatchResults(
        parameters?: MatchResultParameters
    ): Observable<PagedMatchResultResponse> {
        return createPagedRequest<MatchResult, MatchResultParameters>(
            this.http,
            this.baseUrl,
            '',
            parameters
        );
    }

    getMatchResultById(id: string): Observable<MatchResult> {
        return this.http.get<MatchResult>(`${this.baseUrl}/by-id/${id}`);
    }

    getMatchResultByMatchResultId(
        matchResultId: string
    ): Observable<MatchResult> {
        return this.http.get<MatchResult>(
            `${this.baseUrl}/by-result-id/${matchResultId}`
        );
    }
}
