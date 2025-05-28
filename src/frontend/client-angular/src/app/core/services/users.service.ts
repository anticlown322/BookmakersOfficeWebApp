import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { UserParameters } from '../models/user-service/entities/user-parameters.request';
import { UserPagedResponse } from '../models/user-service/responses/user/user-paged.response';
import { UserGetDto } from '../models/user-service/entities/user-get.dto';
import { MetaData } from '../models/shared/interfaces/meta-data';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class UsersService {
    private readonly baseUrl = `${environment.apiUrl}/users`;

    constructor(private http: HttpClient) {}

    getAllUsers(params?: UserParameters): Observable<UserPagedResponse> {
        let httpParams = new HttpParams();

        if (params?.pageNumber) {
            httpParams = httpParams.set('pageNumber', params.pageNumber.toString());
        }

        if (params?.pageSize) {
            httpParams = httpParams.set('pageSize', params.pageSize.toString());
        }

        return this.http.get<UserGetDto[]>(this.baseUrl, {
            params: httpParams,
            observe: 'response'
        }).pipe(
            map(response => {
                const metaData = this.parseMetaData(response.headers.get('X-Pagination'));
                return {
                    items: response.body || [],
                    metaData
                };
            })
        );
    }

    getUserById(id: string): Observable<UserGetDto> {
        return this.http.get<UserGetDto>(`${this.baseUrl}/${id}`);
    }

    getUserByName(username: string): Observable<UserGetDto> {
        return this.http.get<UserGetDto>(`${this.baseUrl}/${username}`);
    }

    deleteUser(username: string): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl}/${username}`);
    }

    private parseMetaData(paginationHeader: string | null): MetaData {
        if (!paginationHeader) {
            return {
                currentPage: 1,
                totalPages: 1,
                pageSize: 10,
                totalCount: 0,
                hasPrevious: false,
                hasNext: false
            };
        }

        try {
            const meta = JSON.parse(paginationHeader);
            return {
                currentPage: meta.CurrentPage,
                totalPages: meta.TotalPages,
                pageSize: meta.PageSize,
                totalCount: meta.TotalCount,
                hasPrevious: meta.HasPrevious,
                hasNext: meta.HasNext
            };
        } catch {
            return {
                currentPage: 1,
                totalPages: 1,
                pageSize: 10,
                totalCount: 0,
                hasPrevious: false,
                hasNext: false
            };
        }
    }

    createUserParams(
        pageNumber: number = 1,
        pageSize: number = 10
    ): UserParameters {
        return {
            pageNumber,
            pageSize
        };
    }
}