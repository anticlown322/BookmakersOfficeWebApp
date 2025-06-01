import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { MetaData } from '../../models/shared/interfaces/meta-data';
import { PagedList } from '../../models/shared/interfaces/paged-list';

export function createPagedRequest<T, P>(
    http: HttpClient,
    baseUrl: string,
    endpoint: string,
    parameters?: P
): Observable<PagedList<T>> {
    let params = new HttpParams();

    if (parameters) {
        if ((parameters as any).pageNumber) {
            params = params.append(
                'pageNumber',
                (parameters as any).pageNumber.toString()
            );
        }
        if ((parameters as any).pageSize) {
            params = params.append(
                'pageSize',
                (parameters as any).pageSize.toString()
            );
        }
    }

    const url = endpoint.trim() == '' ? `${baseUrl}` : `${baseUrl}/${endpoint}`;

    return http
        .get<T[]>(url, {
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