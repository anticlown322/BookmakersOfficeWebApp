import {
    Directive,
    Input,
    OnDestroy,
    OnInit,
    TemplateRef,
    ViewContainerRef,
} from '@angular/core';
import { Role } from '../../core/models/shared/enums/role.enum';
import { AuthService } from '../../core/services/user-service/auth.service';
import { Subscription } from 'rxjs';

@Directive({
    selector: '[appHasRole]',
})
export class HasRoleDirective implements OnInit, OnDestroy {
    @Input() appHasRole: Role[] = [];
    private subscription?: Subscription;

    constructor(
        private templateRef: TemplateRef<any>,
        private viewContainer: ViewContainerRef,
        private authService: AuthService
    ) {}

    ngOnInit() {
        this.subscription = this.authService.authStateChanged$.subscribe(() => {
            this.updateView();
        });
    }

    private updateView() {
        const userRoles = this.authService.getUserRoles();
        const safeUserRoles = Array.isArray(userRoles) ? userRoles : [];

        const hasRole = this.appHasRole.some((requiredRole) =>
            safeUserRoles.includes(requiredRole)
        );

        this.viewContainer.clear();
        if (hasRole) {
            this.viewContainer.createEmbeddedView(this.templateRef);
        }
    }

    ngOnDestroy() {
        this.subscription?.unsubscribe();
    }
}
