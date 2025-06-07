import { Component } from '@angular/core';
import {
    FormGroup,
    FormControl,
    Validators,
    ReactiveFormsModule,
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/user-service/auth.service';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss'],
    imports: [ReactiveFormsModule, RouterLink],
})
export class LoginComponent {
    loginForm = new FormGroup({
        username: new FormControl('', Validators.required),
        password: new FormControl('', Validators.required),
    });

    errorMessage?: string;

    constructor(private authService: AuthService, private router: Router) {}

    ngOnInit() {
        if (this.authService.isAuthenticated()) {
            this.router.navigate(['/']);
        }
    }

    onSubmit() {
        if (this.loginForm.invalid) return;

        const credentials = {
            userName: this.loginForm.value.username!,
            password: this.loginForm.value.password!,
        };

        this.authService.login(credentials).subscribe({
            next: () => {
                this.router.navigate(['/']);
            },
            error: (err) => {
                this.errorMessage = 'Неверные учетные данные';
                console.error('Login error:', err);
            },
        });
    }
}
