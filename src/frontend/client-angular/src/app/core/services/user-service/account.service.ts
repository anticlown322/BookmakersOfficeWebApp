import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { UserProfile } from '../../models/user-service/entities/user-profile.model';

@Injectable({
    providedIn: 'root',
})
export class AccountService {
    private readonly baseUrl = `${environment.apiUrl}/users`;

    constructor(private http: HttpClient) {}

    confirmEmail(username: string): Observable<void> {
        return this.http.post<void>(
            `${this.baseUrl}/${username}/account/confirm-email`,
            {}
        );
    }

    sendConfirmationEmail(username: string): Observable<void> {
        return this.http.post<void>(
            `${this.baseUrl}/${username}/account/send-confirmation-email`,
            {}
        );
    }

    sendResetPasswordEmail(username: string): Observable<void> {
        return this.http.post<void>(
            `${this.baseUrl}/${username}/account/send-reset-email`,
            {}
        );
    }

    resetPassword(
        username: string,
        token: string,
        newPassword: string
    ): Observable<void> {
        return this.http.post<void>(
            `${this.baseUrl}/${username}/account/reset-password`,
            { token, newPassword }
        );
    }

    getUserProfile(username: string): Observable<UserProfile> {
        return this.http.get<UserProfile>(
            `${this.baseUrl}/${username}/account/profile`
        );
    }

    updateUserProfile(
        username: string,
        profile: { firstName: string; lastName: string }
    ): Observable<void> {
        return this.http.put<void>(
            `${this.baseUrl}/${username}/account/profile`,
            profile
        );
    }
}
