import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { TeamParameters } from '../../models/sport-data-service/requests/team/team-parameters.request';
import { PagedTeamResponse } from '../../models/sport-data-service/responses/team/paged-team.response';
import { TeamResponse } from '../../models/sport-data-service/responses/team/team.response';
import { Team } from '../../models/sport-data-service/entities/team/team.model';
import { MetaData } from '../../models/shared/interfaces/meta-data';
import { createPagedRequest } from '../shared/create-paged-request.function';

@Injectable({
    providedIn: 'root',
})
export class TeamService {
    private readonly baseUrl = `${environment.apiUrl}/teams`;

    constructor(private http: HttpClient) {}

    getTeams(parameters?: TeamParameters): Observable<PagedTeamResponse> {
        return createPagedRequest<Team, TeamParameters>(
            this.http,
            this.baseUrl,
            '',
            parameters
        );
    }

    getTeamById(id: string): Observable<TeamResponse> {
        return this.http.get<TeamResponse>(`${this.baseUrl}/by-id/${id}`);
    }

    getTeamByTeamId(teamId: string): Observable<TeamResponse> {
        return this.http.get<TeamResponse>(
            `${this.baseUrl}/by-team-id/${teamId}`
        );
    }
}
