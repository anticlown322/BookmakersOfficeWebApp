import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { TournamentParameters } from '../../models/sport-data-service/requests/tournaments/tournament-parameters.request';
import { PagedTournamentResponse } from '../../models/sport-data-service/responses/tournament/paged-tournament.response';
import { Tournament } from '../../models/sport-data-service/entities/tournament/tournament.model';
import { MetaData } from '../../models/shared/interfaces/meta-data';

@Injectable({
    providedIn: 'root',
})
export class TournamentService {
    private readonly baseUrl = `${environment.apiUrl}/tournaments`;

    constructor(private http: HttpClient) {}

    refreshTournaments(): Observable<void> {
        return this.http.post<void>(this.baseUrl, {});
    }

    getTournaments(
        parameters?: TournamentParameters
    ): Observable<PagedTournamentResponse> {
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
            .get<Tournament[]>(this.baseUrl, {
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

    getTournamentById(id: string): Observable<Tournament> {
        return this.http.get<Tournament>(`${this.baseUrl}/by-id/${id}`);
    }

    getTournamentByTournamentId(tournamentId: string): Observable<Tournament> {
        return this.http.get<Tournament>(
            `${this.baseUrl}/by-tournament-id/${tournamentId}`
        );
    }
}
