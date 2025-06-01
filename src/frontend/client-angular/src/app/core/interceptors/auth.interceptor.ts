import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/user-service/auth.service';
import { catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
    const authService = inject(AuthService);
    const router = inject(Router);
    const token = authService.getCurrentAccessToken();

    if (token && !req.url.includes('/authentication')) {
        req = req.clone({
            setHeaders: {
                Authorization: `Bearer ${token}`,
            },
        });
    }

    return next(req).pipe(
        catchError((error) => {
            if (error.status === 401) {
                authService.clearTokens();
                router.navigate(['/login']);
            } else if (error.status === 403) {
                router.navigate(['/forbidden']);
            }
            return throwError(() => error);
        })
    );
};
