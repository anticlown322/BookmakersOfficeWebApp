import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { MatchParameters } from '../../models/sport-data-service/requests/match/match-parameters.request';
import { PagedMatchResponse } from '../../models/sport-data-service/responses/match/paged-match.response';
import { Match } from '../../models/sport-data-service/entities/match/match.model';
import { MetaData } from '../../models/shared/interfaces/meta-data';
import { createPagedRequest } from '../shared/create-paged-request.function';

@Injectable({
    providedIn: 'root',
})
export class MatchService {
    private readonly baseUrl = `${environment.apiUrl}/matches`;

    constructor(private http: HttpClient) {}

    getMatchResults(
        parameters?: MatchParameters
    ): Observable<PagedMatchResponse> {
        return createPagedRequest<Match, MatchParameters>(
            this.http,
            this.baseUrl,
            '',
            parameters
        );
    }

    getMatchById(id: string): Observable<Match> {
        return this.http.get<Match>(`${this.baseUrl}/by-id/${id}`);
    }

    getMatchByMatchId(matchId: string): Observable<Match> {
        return this.http.get<Match>(`${this.baseUrl}/by-match-id/${matchId}`);
    }
}
