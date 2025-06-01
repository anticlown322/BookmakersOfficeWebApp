import { Injectable } from '@angular/core';
import {
    ActivatedRouteSnapshot,
    CanActivate,
    Router,
    RouterStateSnapshot,
} from '@angular/router';
import { AuthService } from './auth.service';
import { Observable } from 'rxjs';
import { Role } from '../../models/shared/enums/role.enum';

@Injectable({ providedIn: 'root' })
export class RoleGuard implements CanActivate {
    constructor(private authService: AuthService, private router: Router) {}

    canActivate(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ): boolean | Observable<boolean> | Promise<boolean> {
        const requiredRoles = route.data['roles'] as Role[];
        const userRoles = this.authService.getUserRoles();

        if (
            !userRoles ||
            !requiredRoles.some((role) => userRoles.includes(role))
        ) {
            this.router.navigate(['/login']);
            return false;
        }

        return true;
    }
}
