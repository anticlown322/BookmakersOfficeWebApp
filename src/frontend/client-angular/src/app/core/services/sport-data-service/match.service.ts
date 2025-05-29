import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { MatchParameters } from '../../models/sport-data-service/requests/match/match-parameters.request';
import { PagedMatchResponse } from '../../models/sport-data-service/responses/match/paged-match.response';
import { Match } from '../../models/sport-data-service/entities/match/match.model';
import { MetaData } from '../../models/shared/interfaces/meta-data';

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

        return this.http
            .get<Match[]>(this.baseUrl, {
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

    getMatchById(id: string): Observable<Match> {
        return this.http.get<Match>(`${this.baseUrl}/by-id/${id}`);
    }

    getMatchByMatchId(matchId: string): Observable<Match> {
        return this.http.get<Match>(`${this.baseUrl}/by-match-id/${matchId}`);
    }
}
