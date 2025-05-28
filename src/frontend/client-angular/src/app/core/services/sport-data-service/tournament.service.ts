import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { TournamentParameters } from '../../models/sport-data-service/requests/tournaments/tournament-parameters.request';
import { PagedTournamentResponse } from '../../models/sport-data-service/responses/tournament/paged-tournament.response';
import { Tournament } from '../../models/sport-data-service/entities/tournament/tournament.model';

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

        return this.http.get<PagedTournamentResponse>(this.baseUrl, { params });
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
