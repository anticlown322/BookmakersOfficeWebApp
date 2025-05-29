import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { TeamParameters } from '../../models/sport-data-service/requests/team/team-parameters.request';
import { PagedTeamResponse } from '../../models/sport-data-service/responses/team/paged-team.response';
import { TeamResponse } from '../../models/sport-data-service/responses/team/team.response';
import { Team } from '../../models/sport-data-service/entities/team/team.model';
import { MetaData } from '../../models/shared/interfaces/meta-data';

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

        return this.http
            .get<Team[]>(this.baseUrl, { params, observe: 'response' })
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

    getTeamById(id: string): Observable<TeamResponse> {
        return this.http.get<TeamResponse>(`${this.baseUrl}/by-id/${id}`);
    }

    getTeamByTeamId(teamId: string): Observable<TeamResponse> {
        return this.http.get<TeamResponse>(
            `${this.baseUrl}/by-team-id/${teamId}`
        );
    }
}
