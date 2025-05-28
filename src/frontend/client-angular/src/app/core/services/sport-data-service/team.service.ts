import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { TeamParameters } from '../../models/sport-data-service/requests/team/team-parameters.request';
import { Team } from '../../models/sport-data-service/entities/team/team.model';
import { PagedTeamResponse } from '../../models/sport-data-service/responses/team/paged-team.response';

@Injectable({
    providedIn: 'root',
})
export class TeamService {
    private readonly baseUrl = `${environment.apiUrl}/teams`;

    constructor(private http: HttpClient) {}

    getTeams(parameters?: TeamParameters): Observable<PagedTeamResponse> {
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

        return this.http.get<PagedTeamResponse>(this.baseUrl, { params });
    }

    getTeamById(id: string): Observable<Team> {
        return this.http.get<Team>(`${this.baseUrl}/by-id/${id}`);
    }

    getTeamByTeamId(teamId: string): Observable<Team> {
        return this.http.get<Team>(`${this.baseUrl}/by-team-id/${teamId}`);
    }
}
