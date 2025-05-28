import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { MatchParameters } from '../../models/sport-data-service/requests/match/match-parameters.request';
import { PagedMatchResponse } from '../../models/sport-data-service/responses/match/paged-match.response';
import { Match } from '../../models/sport-data-service/entities/match/match.model';

@Injectable({
    providedIn: 'root',
})
export class MatchService {
    private readonly baseUrl = `${environment.apiUrl}/matches`;

    constructor(private http: HttpClient) {}

    getMatches(parameters?: MatchParameters): Observable<PagedMatchResponse> {
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

        return this.http.get<PagedMatchResponse>(this.baseUrl, { params });
    }

    getMatchById(id: string): Observable<Match> {
        return this.http.get<Match>(`${this.baseUrl}/by-id/${id}`);
    }

    getMatchByMatchId(matchId: string): Observable<Match> {
        return this.http.get<Match>(`${this.baseUrl}/by-match-id/${matchId}`);
    }
}
