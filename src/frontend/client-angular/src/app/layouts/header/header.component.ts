import { Component, OnDestroy } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/user-service/auth.service';
import { Subscription } from 'rxjs';
import { Role } from '../../core/models/shared/enums/role.enum';
import { HasRoleDirective } from '../../shared/ui/has-role.directive';

@Component({
    selector: 'app-header',
    templateUrl: './header.component.html',
    styleUrls: ['./header.component.scss'],
    imports: [RouterLink, HasRoleDirective],
})
export class HeaderComponent implements OnDestroy {
    Role = Role;
    isProfileMenuOpen = false;
    currentUsername: string | null = null;
    private authSub: Subscription;

    constructor(public authService: AuthService, private router: Router) {
        this.authSub = this.authService.authStateChanged$.subscribe(() => {
            this.updateUserData();
        });
    }

    ngOnDestroy(): void {
        this.authSub.unsubscribe();
    }

    private updateUserData(): void {
        this.currentUsername = this.authService.getCurrentUsername();
    }

    toggleProfileMenu(event: Event): void {
        event.stopPropagation();
        this.isProfileMenuOpen = !this.isProfileMenuOpen;
    }

    logout(): void {
        this.authService.logout().subscribe({
            next: () => {
                this.handleLogoutSuccess();
            },
            error: () => {
                this.handleLogoutSuccess();
            },
        });
    }

    private handleLogoutSuccess(): void {
        this.isProfileMenuOpen = false;
        this.router.navigate(['/']);
    }

    onDocumentClick(): void {
        if (this.isProfileMenuOpen) {
            this.isProfileMenuOpen = false;
        }
    }
}
