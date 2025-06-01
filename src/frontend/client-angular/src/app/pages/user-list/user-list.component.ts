import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { UsersService } from '../../core/services/user-service/users.service';
import { AuthService } from '../../core/services/user-service/auth.service';
import { MetaData } from '../../core/models/shared/interfaces/meta-data';
import { Role } from '../../core/models/shared/enums/role.enum';
import { UserGet } from '../../core/models/user-service/entities/user-get.model';

@Component({
    selector: 'app-user-list',
    templateUrl: './user-list.component.html',
    styleUrl: './user-list.component.scss',
    standalone: true,
    imports: [CommonModule],
})
export class UserListComponent implements OnInit {
    users: UserGet[] = [];
    isLoading = true;
    isTransitioning = false;
    currentPage = 1;
    pageSize = 10;
    totalPages = 0;
    totalCount = 0;
    hasPrevious = false;
    hasNext = false;
    MIN_LOADING_DISPLAY_TIME = 500;

    usersCache: Record<number, { items: UserGet[]; metaData: MetaData }> = {};

    constructor(
        private usersService: UsersService,
        private authService: AuthService
    ) {}

    ngOnInit(): void {
        if (this.authService.isAuthenticated() && this.authService.hasAnyRole([Role.Bookmaker, Role.Administrator])) {
            this.loadInitialUsers();
        }
    }

    loadInitialUsers(): void {
        const startTime = Date.now();

        this.isLoading = true;
        this.isTransitioning = true;
        this.users = [];

        this.usersService
            .getAllUsers({
                pageNumber: this.currentPage,
                pageSize: this.pageSize,
            })
            .subscribe({
                next: ({ items, metaData }) => {
                    const elapsed = Date.now() - startTime;
                    const remainingTime = Math.max(
                        0,
                        this.MIN_LOADING_DISPLAY_TIME - elapsed
                    );

                    setTimeout(() => {
                        this.cacheData(this.currentPage, items, metaData);
                        this.isLoading = false;
                        this.isTransitioning = false;
                    }, remainingTime);
                },
                error: (error) => {
                    console.error('Error loading users:', error);
                    this.isLoading = false;
                    this.isTransitioning = false;
                },
            });
    }

    loadUsers(page: number): void {
        if (this.isTransitioning || page === this.currentPage) return;

        const startTime = Date.now();
        this.isTransitioning = true;
        this.users = [];

        if (this.usersCache[page]) {
            this.applyCachedData(page);
            return;
        }

        this.usersService
            .getAllUsers({
                pageNumber: page,
                pageSize: this.pageSize,
            })
            .subscribe({
                next: ({ items, metaData }) => {
                    console.log(metaData.CurrentPage);
                    console.log(metaData.TotalPages);

                    const elapsed = Date.now() - startTime;
                    const remainingTime = Math.max(
                        0,
                        this.MIN_LOADING_DISPLAY_TIME - elapsed
                    );

                    setTimeout(() => {
                        this.cacheData(page, items, metaData);
                        this.isTransitioning = false;
                    }, remainingTime);
                },
                error: (error) => {
                    console.error('Error loading users:', error);
                    this.isTransitioning = false;
                },
            });
    }

    private applyCachedData(page: number): void {
        const cached = this.usersCache[page];
        this.users = cached.items;
        this.currentPage = page;
        this.totalPages = cached.metaData?.TotalPages || 0;
        this.totalCount = cached.metaData?.TotalCount || 0;
        this.hasPrevious = cached.metaData?.HasPrevious || false;
        this.hasNext = cached.metaData?.HasNext || false;
        this.isLoading = false;
        setTimeout(() => {
            this.isTransitioning = false;
        }, 50);
    }

    private cacheData(page: number, items: UserGet[], metaData: MetaData): void {
        this.usersCache[page] = { items, metaData };
        this.applyCachedData(page);
    }

    trackByUserName(index: number, user: UserGet): string {
        return user.userName;
    }

    onPageChange(page: number): void {
        if (page < 1 || page > this.totalPages) return;
        this.loadUsers(page);
    }
}
