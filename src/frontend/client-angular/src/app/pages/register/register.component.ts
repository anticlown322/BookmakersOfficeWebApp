import { Component } from '@angular/core';
import {
    FormGroup,
    FormControl,
    Validators,
    ReactiveFormsModule,
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Role } from '../../core/models/shared/enums/role.enum';
import { AuthService } from '../../core/services/user-service/auth.service';
import { UserRegistrationRequest } from '../../core/models/user-service/requests/auth/register.request';
import { passwordValidator } from '../../shared/validators/password.validator';
import { phoneNumberValidator } from '../../shared/validators/phone-number.validator';
import { emailValidator } from '../../shared/validators/email.validator';
import { passwordMatchValidator } from '../../shared/validators/password-match.validator';
import { RoleService } from '../../core/services/shared/role.service';

@Component({
    selector: 'app-register',
    templateUrl: './register.component.html',
    styleUrls: ['./register.component.scss'],
    imports: [ReactiveFormsModule, RouterLink],
})
export class RegisterComponent {
    roles: { value: Role; label: string }[];
    showPassword = false;
    registrationError?: string;

    registerForm = new FormGroup(
        {
            firstName: new FormControl('', [
                Validators.required,
                Validators.minLength(2),
            ]),
            lastName: new FormControl('', [
                Validators.required,
                Validators.minLength(2),
            ]),
            userName: new FormControl('', [
                Validators.required,
                Validators.minLength(4),
            ]),
            email: new FormControl('', [Validators.required, emailValidator]),
            phoneNumber: new FormControl('', [phoneNumberValidator]),
            password: new FormControl('', [
                Validators.required,
                Validators.minLength(8),
                passwordValidator,
            ]),
            confirmPassword: new FormControl('', [Validators.required]),
            role: new FormControl<Role>(Role.Gambler, Validators.required),
        },
        { validators: passwordMatchValidator }
    );

    constructor(
        private authService: AuthService,
        private router: Router,
        private roleService: RoleService
    ) {
        this.roles = this.roleService.getTranslatedRoles();
    }

    togglePasswordVisibility(): void {
        this.showPassword = !this.showPassword;
    }

    onSubmit(): void {
        if (this.registerForm.invalid) return;

        const formData = this.registerForm.value;
        const registrationData: UserRegistrationRequest = {
            firstName: formData.firstName!,
            lastName: formData.lastName!,
            userName: formData.userName!,
            phoneNumber: formData.phoneNumber!,
            email: formData.email!,
            password: formData.password!,
            roles: [formData.role!] as Role[],
        };

        this.authService.register(registrationData).subscribe({
            next: () => {
                this.router.navigate(['/login'], {
                    queryParams: { registered: true },
                });
            },
            error: (err) => {
                console.error('Registration error:', err);
                this.registrationError =
                    err.error?.message || 'Ошибка регистрации';
            },
        });
    }

    get passwordControl() {
        return this.registerForm.get('password');
    }

    hasUpperCase(value: string): boolean {
        return /[A-Z]/.test(value);
    }

    hasLowerCase(value: string): boolean {
        return /[a-z]/.test(value);
    }

    hasNumber(value: string): boolean {
        return /\d/.test(value);
    }
}
