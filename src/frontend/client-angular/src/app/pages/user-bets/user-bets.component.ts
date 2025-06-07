import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Bet } from '../../core/models/betting-serivce/entities/bet/bet.model';
import { BetsService } from '../../core/services/betting-service/bets.service';
import { AuthService } from '../../core/services/user-service/auth.service';
import { BetStatus } from '../../core/models/betting-serivce/entities/bet/bet-status.enum';
import { MetaData } from '../../core/models/shared/interfaces/meta-data';

@Component({
    selector: 'app-user-bets',
    templateUrl: './user-bets.component.html',
    styleUrls: ['./user-bets.component.scss'],
    standalone: true,
    imports: [CommonModule],
})
export class UserBetsComponent implements OnInit {
    currentUsername: string | null = null;
    bets: Bet[] = [];
    isLoading = true;
    isTransitioning = false;
    currentPage = 1;
    pageSize = 10;
    totalPages = 0;
    totalCount = 0;
    hasPrevious = false;
    hasNext = false;
    MIN_LOADING_DISPLAY_TIME = 500;

    betsCache: Record<number, { items: Bet[]; metaData: MetaData }> = {};

    constructor(
        private betsService: BetsService,
        private authService: AuthService
    ) {}

    ngOnInit(): void {
        this.currentUsername = this.authService.getCurrentUsername();
        if (this.currentUsername && this.authService.isAuthenticated()) {
            this.loadInitialBets();
        }
    }

    loadInitialBets(): void {
        const startTime = Date.now();

        this.isLoading = true;
        this.isTransitioning = true;
        this.bets = [];

        this.betsService
            .getUserBets({
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
                    console.error('Error loading bets:', error);
                    this.isLoading = false;
                    this.isTransitioning = false;
                },
            });
    }

    loadBets(page: number): void {
        if (this.isTransitioning || page === this.currentPage) return;

        const startTime = Date.now();
        this.isTransitioning = true;
        this.bets = [];

        if (this.betsCache[page]) {
            this.applyCachedData(page);
            return;
        }

        this.betsService
            .getUserBets({
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
                    console.error('Error loading bets:', error);
                    this.isTransitioning = false;
                },
            });
    }

    private applyCachedData(page: number): void {
        const cached = this.betsCache[page];
        this.bets = cached.items;
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

    private cacheData(page: number, items: Bet[], metaData: MetaData): void {
        this.betsCache[page] = { items, metaData };
        this.applyCachedData(page);
    }

    trackByBetId(index: number, bet: Bet): string {
        return bet.id;
    }

    getBetStatusName(status: BetStatus): string {
        switch (status) {
            case BetStatus.Active:
                return 'active';
            case BetStatus.Won:
                return 'won';
            case BetStatus.Lost:
                return 'lost';
            case BetStatus.Pending:
                return 'pending';
            case BetStatus.Cancelled:
                return 'canceled';
            case BetStatus.Rejected:
                return 'rejected';
            case BetStatus.Refunded:
                return 'refunded';
            case BetStatus.Validating:
                return 'validating';
            default:
                return '';
        }
    }

    onPageChange(page: number): void {
        if (page < 1 || page > this.totalPages) return;
        this.loadBets(page);
    }
}
