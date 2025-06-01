import { Component, OnInit } from '@angular/core';
import { Observable, of, switchMap } from 'rxjs';
import { CommonModule } from '@angular/common';
import { UserProfile } from '../../core/models/user-service/entities/user-profile.model';
import { AuthService } from '../../core/services/user-service/auth.service';
import { AccountService } from '../../core/services/user-service/account.service';
import { FormsModule } from '@angular/forms';
import { UserProfileUpdateRequest } from '../../core/models/user-service/requests/account/user-profile-update.request';
import { PasswordResetRequest } from '../../core/models/user-service/requests/account/password-reset.request';

@Component({
    selector: 'app-profile',
    templateUrl: './profile.component.html',
    styleUrls: ['./profile.component.scss'],
    standalone: true,
    imports: [CommonModule, FormsModule],
})
export class ProfileComponent implements OnInit {
    isAuthenticated: boolean = false;
    profile$: Observable<UserProfile | null> = of(null);
    currentUsername: string | null = null;
    editMode = false;
    updateProfileRequest: UserProfileUpdateRequest = {
        firstName: '',
        lastName: '',
    };
    resetPasswordRequest: PasswordResetRequest = { token: '', newPassword: '' };
    confirmPassword: string = '';
    resetPasswordMode = false;
    resetPasswordSuccess = false;
    loading = false;
    errorMessage: string | null = null;
    successMessage: string | null = null;

    constructor(
        private authService: AuthService,
        private accountService: AccountService
    ) {}

    ngOnInit(): void {
        this.isAuthenticated = this.authService.isAuthenticated();
        this.currentUsername = this.authService.getCurrentUsername();

        if (this.isAuthenticated && this.currentUsername) {
            this.loadProfile();
        }
    }

    loadProfile(): void {
        if (!this.currentUsername) return;

        this.loading = true;
        this.profile$ = this.accountService.getUserProfile(
            this.currentUsername
        );
        this.profile$.subscribe({
            next: (profile) => {
                if (profile) {
                    this.updateProfileRequest = {
                        firstName: profile.firstName,
                        lastName: profile.lastName,
                    };
                }
                this.loading = false;
            },
            error: () => {
                this.loading = false;
                this.errorMessage = 'Ошибка загрузки профиля';
            },
        });
    }

    sendConfirmationEmail(): void {
        if (!this.currentUsername) return;

        this.loading = true;
        this.accountService
            .sendConfirmationEmail(this.currentUsername)
            .subscribe({
                next: () => {
                    this.successMessage = 'Письмо подтверждения отправлено';
                    this.loading = false;
                },
                error: () => {
                    this.errorMessage = 'Ошибка отправки письма подтверждения';
                    this.loading = false;
                },
            });
    }

    confirmEmail(): void {
        if (!this.currentUsername) return;

        this.loading = true;
        this.accountService.confirmEmail(this.currentUsername).subscribe({
            next: () => {
                this.successMessage = 'Email успешно подтвержден';
                this.loadProfile();
                this.loading = false;
            },
            error: () => {
                this.errorMessage = 'Ошибка подтверждения email';
                this.loading = false;
            },
        });
    }

    sendResetPasswordEmail(): void {
        if (!this.currentUsername) return;

        this.loading = true;
        this.accountService
            .sendResetPasswordEmail(this.currentUsername)
            .subscribe({
                next: () => {
                    this.successMessage = 'Письмо для сброса пароля отправлено';
                    this.loading = false;
                },
                error: () => {
                    this.errorMessage =
                        'Ошибка отправки письма для сброса пароля';
                    this.loading = false;
                },
            });
    }

    resetPassword(): void {
        if (
            !this.currentUsername ||
            this.resetPasswordRequest.newPassword !== this.confirmPassword
        ) {
            this.errorMessage = 'Пароли не совпадают';
            return;
        }

        console.log(this.resetPasswordRequest);

        this.loading = true;
        this.accountService
            .resetPassword(this.currentUsername, this.resetPasswordRequest)
            .subscribe({
                next: () => {
                    this.successMessage = 'Пароль успешно изменен';
                    this.resetPasswordMode = false;
                    this.resetPasswordSuccess = true;
                    this.loading = false;
                },
                error: () => {
                    this.errorMessage = 'Ошибка сброса пароля';
                    this.loading = false;
                },
            });
    }

    updateProfile(): void {
        if (!this.currentUsername) return;

        this.loading = true;
        this.accountService
            .updateUserProfile(this.currentUsername, this.updateProfileRequest)
            .subscribe({
                next: () => {
                    this.successMessage = 'Профиль успешно обновлен';
                    this.editMode = false;
                    this.loadProfile();
                    this.loading = false;
                },
                error: () => {
                    this.errorMessage = 'Ошибка обновления профиля';
                    this.loading = false;
                },
            });
    }

    clearMessages(): void {
        this.errorMessage = null;
        this.successMessage = null;
    }
}
