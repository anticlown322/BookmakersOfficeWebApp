import { Component, OnInit } from '@angular/core';
import { MetaData } from '../../core/models/shared/interfaces/meta-data';
import { AuthService } from '../../core/services/user-service/auth.service';
import { CommonModule } from '@angular/common';
import { Role } from '../../core/models/shared/enums/role.enum';
import { Payout } from '../../core/models/betting-serivce/entities/payout/payout.model';
import { PayoutsService } from '../../core/services/betting-service/payouts.service';
import { PayoutStatus } from '../../core/models/betting-serivce/entities/payout/payout-status.enum';

@Component({
    selector: 'app-payout-list',
    templateUrl: './payout-list.component.html',
    styleUrl: './payout-list.component.scss',
    standalone: true,
    imports: [CommonModule],
})
export class PayoutListComponent implements OnInit {
    payouts: Payout[] = [];
    isLoading = true;
    isTransitioning = false;
    currentPage = 1;
    pageSize = 10;
    totalPages = 0;
    totalCount = 0;
    hasPrevious = false;
    hasNext = false;
    MIN_LOADING_DISPLAY_TIME = 500;

    payoutsCache: Record<number, { items: Payout[]; metaData: MetaData }> = {};

    constructor(
        private payoutsService: PayoutsService,
        private authService: AuthService
    ) {}

    ngOnInit(): void {
        if (this.authService.isAuthenticated() && this.authService.hasAnyRole([Role.Bookmaker, Role.Administrator])) {
            this.loadInitialPayouts();
        }
    }

    loadInitialPayouts(): void {
        const startTime = Date.now();

        this.isLoading = true;
        this.isTransitioning = true;
        this.payouts = [];

        this.payoutsService
            .getAllPayouts({
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
                    console.error('Error loading payouts:', error);
                    this.isLoading = false;
                    this.isTransitioning = false;
                },
            });
    }

    loadPayouts(page: number): void {
        if (this.isTransitioning || page === this.currentPage) return;

        const startTime = Date.now();
        this.isTransitioning = true;
        this.payouts = [];

        if (this.payoutsCache[page]) {
            this.applyCachedData(page);
            return;
        }

        this.payoutsService
            .getAllPayouts({
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
                    console.error('Error loading payouts:', error);
                    this.isTransitioning = false;
                },
            });
    }

    private applyCachedData(page: number): void {
        const cached = this.payoutsCache[page];
        this.payouts = cached.items;
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

    private cacheData(page: number, items: Payout[], metaData: MetaData): void {
        this.payoutsCache[page] = { items, metaData };
        this.applyCachedData(page);
    }

    trackByPayoutId(index: number, payout: Payout): string {
        return payout.id;
    }

    getPayoutStatusName(status: PayoutStatus): string {
        switch (status) {
            case PayoutStatus.Completed:
                return 'won';
            case PayoutStatus.Failed:
                return 'lost';
            case PayoutStatus.Pending:
                return 'pending';
            default:
                return '';
        }
    }

    onPageChange(page: number): void {
        if (page < 1 || page > this.totalPages) return;
        this.loadPayouts(page);
    }
}
