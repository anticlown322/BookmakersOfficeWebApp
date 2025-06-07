import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { UserProfile } from '../../models/user-service/entities/user-profile.model';
import { PasswordResetRequest } from '../../models/user-service/requests/account/password-reset.request';
import { UserProfileUpdateRequest } from '../../models/user-service/requests/account/user-profile-update.request';

@Injectable({
    providedIn: 'root',
})
export class AccountService {
    private readonly baseUrl = `${environment.apiUrl}/users`;

    constructor(private http: HttpClient) {}

    confirmEmail(username: string): Observable<void> {
        return this.http.get<void>(
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
        request: PasswordResetRequest
    ): Observable<void> {
        return this.http.post<void>(
            `${this.baseUrl}/${username}/account/reset-password`,
            request
        );
    }

    getUserProfile(username: string): Observable<UserProfile> {
        return this.http.get<UserProfile>(
            `${this.baseUrl}/${username}/account/profile`
        );
    }

    updateUserProfile(
        username: string,
        request: UserProfileUpdateRequest
    ): Observable<void> {
        return this.http.put<void>(
            `${this.baseUrl}/${username}/account/profile`,
            request
        );
    }
}
