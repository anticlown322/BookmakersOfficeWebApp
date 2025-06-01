import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { TournamentParameters } from '../../models/sport-data-service/requests/tournaments/tournament-parameters.request';
import { PagedTournamentResponse } from '../../models/sport-data-service/responses/tournament/paged-tournament.response';
import { Tournament } from '../../models/sport-data-service/entities/tournament/tournament.model';
import { MetaData } from '../../models/shared/interfaces/meta-data';
import { createPagedRequest } from '../shared/create-paged-request.function';

@Injectable({
    providedIn: 'root',
})
export class TournamentService {
    private readonly baseUrl = `${environment.apiUrl}/tournaments`;

    constructor(private http: HttpClient) {}

    getTournaments(
        parameters?: TournamentParameters
    ): Observable<PagedTournamentResponse> {
        return createPagedRequest<Tournament, TournamentParameters>(
            this.http,
            this.baseUrl,
            '',
            parameters
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
