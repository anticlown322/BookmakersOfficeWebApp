import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { UserParameters } from '../../models/user-service/requests/users/user-parameters.request';
import { PagedUserResponse } from '../../models/user-service/responses/user/paged-user.response';
import { UserGet } from '../../models/user-service/entities/user-get.model';

@Injectable({ providedIn: 'root' })
export class UsersService {
    private readonly baseUrl = `${environment.apiUrl}/users`;

    constructor(private http: HttpClient) {}

    getAllUsers(parameters?: UserParameters): Observable<PagedUserResponse> {
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

        return this.http.get<PagedUserResponse>(this.baseUrl, { params });
    }

    getUserById(id: string): Observable<UserGet> {
        return this.http.get<UserGet>(`${this.baseUrl}/${id}`);
    }

    getUserByName(username: string): Observable<UserGet> {
        return this.http.get<UserGet>(`${this.baseUrl}/${username}`);
    }

    deleteUser(username: string): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl}/${username}`);
    }
}
