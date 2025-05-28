import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { MatchResultParameters } from '../../models/sport-data-service/requests/match-result/match-result-parameters.request';
import { PagedMatchResultResponse } from '../../models/sport-data-service/responses/match-result/paged-match-result.response';
import { MatchResult } from '../../models/sport-data-service/entities/match-result/match-result.model';

@Injectable({
    providedIn: 'root',
})
export class MatchResultService {
    private readonly baseUrl = `${environment.apiUrl}/match-results`;

    constructor(private http: HttpClient) {}

    getMatchResults(
        parameters?: MatchResultParameters
    ): Observable<PagedMatchResultResponse> {
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

        return this.http.get<PagedMatchResultResponse>(this.baseUrl, {
            params,
        });
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
