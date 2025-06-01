import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { finalize } from 'rxjs/operators';
import { AccountService } from '../../core/services/user-service/account.service';

@Component({
    selector: 'app-email-confirmation',
    templateUrl: './email-confirmation.component.html',
    styleUrls: ['./email-confirmation.component.scss'],
})
export class EmailConfirmationComponent implements OnInit {
    username: string = '';
    isLoading: boolean = true;
    isSuccess: boolean = false;
    errorMessage: string = '';

    constructor(
        private route: ActivatedRoute,
        private accountService: AccountService
    ) {}

    ngOnInit(): void {
        this.username = this.route.snapshot.paramMap.get('username') || '';
        this.confirmEmail();
    }

    confirmEmail(): void {
        this.accountService
            .confirmEmail(this.username)
            .pipe(finalize(() => (this.isLoading = false)))
            .subscribe({
                next: () => (this.isSuccess = true),
                error: (err) => {
                    this.isSuccess = false;
                    this.errorMessage =
                        err.error?.message ||
                        'Произошла ошибка при подтверждении email';
                },
            });
    }
}
