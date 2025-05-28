import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subject, catchError, map, throwError } from 'rxjs';
import { CookieService } from 'ngx-cookie-service';
import { jwtDecode } from 'jwt-decode';
import { environment } from '../../../../environments/environment';
import { UserRegistrationRequest } from '../../models/user-service/requests/auth/register.request';
import { UserLoginRequest } from '../../models/user-service/requests/auth/login.request';
import { TokensGetResponse } from '../../models/user-service/responses/auth/login.response';
import { TokensRefreshRequest } from '../../models/user-service/requests/auth/refresh-token.request';
import { UserLogoutRequest } from '../../models/user-service/requests/auth/logout.request';

@Injectable({ providedIn: 'root' })
export class AuthService {
    private readonly baseURL = `${environment.apiUrl}/authentication`;
    private authStateChangeSubject = new Subject<boolean>();
    public authStateChanged$ = this.authStateChangeSubject.asObservable();

    constructor(
        private http: HttpClient,
        private cookieService: CookieService
    ) {}

    register(userData: UserRegistrationRequest): Observable<void> {
        return this.http.post<void>(`${this.baseURL}/register`, userData);
    }

    login(credentials: UserLoginRequest): Observable<TokensGetResponse> {
        return this.http
            .post<TokensGetResponse>(`${this.baseURL}/login`, credentials)
            .pipe(
                map((response) => {
                    this.storeTokens(response);
                    this.notifyAuthStateChange(true);
                    return response;
                })
            );
    }

    refreshTokens(tokens: TokensRefreshRequest): Observable<string> {
        return this.http.post<string>(`${this.baseURL}/refresh`, tokens).pipe(
            map((newAccessToken) => {
                this.storeRefreshedToken(newAccessToken);
                return newAccessToken;
            }),
            catchError((error) => {
                this.clearTokens();
                return throwError(() => error);
            })
        );
    }

    logout(logoutData: UserLogoutRequest): Observable<void> {
        return this.http.post<void>(`${this.baseURL}/logout`, logoutData).pipe(
            map(() => {
                this.clearTokens();
                this.notifyAuthStateChange(false);
            })
        );
    }

    private storeTokens(tokens: TokensGetResponse): void {
        this.cookieService.set('accessToken', tokens.accessToken, {
            expires: this.getTokenExpiry(tokens.accessToken),
            path: '/',
            secure: true,
            sameSite: 'Strict',
        });

        this.cookieService.set('refreshToken', tokens.refreshToken, {
            expires: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000), // 7 days
            path: '/',
            secure: true,
            sameSite: 'Strict',
        });
    }

    private storeRefreshedToken(accessToken: string): void {
        this.cookieService.set('accessToken', accessToken, {
            expires: this.getTokenExpiry(accessToken),
            path: '/',
            secure: true,
            sameSite: 'Strict',
        });
    }

    public clearTokens(): void {
        this.cookieService.delete('accessToken', '/');
        this.cookieService.delete('refreshToken', '/');
    }

    private getTokenExpiry(token: string): Date {
        const decoded = jwtDecode(token);
        return new Date(decoded.exp! * 1000);
    }

    private notifyAuthStateChange(isAuthenticated: boolean): void {
        this.authStateChangeSubject.next(isAuthenticated);
    }

    getCurrentAccessToken(): string | null {
        return this.cookieService.get('accessToken') || null;
    }

    getCurrentRefreshToken(): string | null {
        return this.cookieService.get('refreshToken') || null;
    }

    isAuthenticated(): boolean {
        try {
            const token = this.getCurrentAccessToken();
            if (!token) return false;

            const decoded = jwtDecode(token);
            return Date.now() < decoded.exp! * 1000;
        } catch {
            return false;
        }
    }

    getTokenClaims<T>(claim: string): T | null {
        const token = this.getCurrentAccessToken();
        if (!token) return null;

        try {
            const decoded = jwtDecode<any>(token);
            return decoded[claim] || null;
        } catch {
            return null;
        }
    }

    getCurrentUsername(): string | undefined {
        const token = this.getCurrentAccessToken();
        if (!token) return undefined;

        try {
            const decoded = jwtDecode<{ username?: string }>(token);
            return decoded.username;
        } catch {
            return undefined;
        }
    }
}
