import { Component, OnInit } from '@angular/core';
import { Observable, of, switchMap } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CurrentBalanceResponse } from '../../core/models/user-service/responses/balance/current-balance.response';
import { DepositRequest } from '../../core/models/user-service/requests/balance/deposit.request';
import { WithdrawRequest } from '../../core/models/user-service/requests/balance/withdraw.request';
import { AuthService } from '../../core/services/user-service/auth.service';
import { BalanceService } from '../../core/services/user-service/balance.service';
import {
    OperationType,
    Transaction,
    TransactionStatus,
} from '../../core/models/user-service/entities/transaction.model';
import { MetaData } from '../../core/models/shared/interfaces/meta-data';

@Component({
    selector: 'app-balance',
    templateUrl: './balance.component.html',
    styleUrl: './balance.component.scss',
    standalone: true,
    imports: [CommonModule, FormsModule],
})
export class BalanceComponent implements OnInit {
    currentUsername: string | null = null;
    balanceData$: Observable<CurrentBalanceResponse | null> = of(null);

    transactions: Transaction[] = [];
    isLoading = true;
    isTransitioning = false;
    currentPage = 1;
    pageSize = 10;
    totalPages = 0;
    totalCount = 0;
    hasPrevious = false;
    hasNext = false;
    MIN_LOADING_DISPLAY_TIME = 500;

    transactionsCache: Record<
        number,
        { items: Transaction[]; metaData: MetaData }
    > = {};

    depositForm: DepositRequest = { amount: 0 };
    withdrawForm: WithdrawRequest = { amount: 0, confirmationCode: '' };
    activeTab: 'deposit' | 'withdraw' | null = null;

    errorMessage: string | null = null;
    successMessage: string | null = null;

    constructor(
        private authService: AuthService,
        private balanceService: BalanceService
    ) {}

    ngOnInit(): void {
        this.currentUsername = this.authService.getCurrentUsername();
        if (this.currentUsername) {
            this.loadBalance();
            this.loadInitialTransactions();
        }
    }

    loadBalance(): void {
        this.balanceData$ = this.balanceService.getCurrentBalance(
            this.currentUsername!
        );
    }

    loadInitialTransactions(): void {
        const startTime = Date.now();

        this.isLoading = true;
        this.isTransitioning = true;
        this.transactions = [];

        this.balanceService
            .getTransactionHistory(this.currentUsername!, {
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
                    console.error('Error loading transactions:', error);
                    this.isLoading = false;
                    this.isTransitioning = false;
                    this.errorMessage = 'Ошибка загрузки транзакций';
                },
            });
    }

    loadTransactions(page: number = 1): void {
        if (this.isTransitioning || page === this.currentPage) return;

        const startTime = Date.now();
        this.isTransitioning = true;
        this.transactions = [];

        if (this.transactionsCache[page]) {
            this.applyCachedData(page);
            return;
        }

        this.balanceService
            .getTransactionHistory(this.currentUsername!, {
                pageNumber: page,
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
                        this.cacheData(page, items, metaData);
                        this.isTransitioning = false;
                    }, remainingTime);
                },
                error: (error) => {
                    console.error('Error loading transactions:', error);
                    this.isTransitioning = false;
                    this.errorMessage = 'Ошибка загрузки транзакций';
                },
            });
    }

    private applyCachedData(page: number): void {
        const cached = this.transactionsCache[page];
        this.transactions = cached.items;
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

    private cacheData(
        page: number,
        items: Transaction[],
        metaData: MetaData
    ): void {
        this.transactionsCache[page] = { items, metaData };
        this.applyCachedData(page);
    }

    trackByTransactionId(index: number, transaction: Transaction): string {
        return transaction.transactionId;
    }

    onPageChange(page: number): void {
        if (page < 1 || page > this.totalPages) return;
        this.loadTransactions(page);
    }

    deposit(): void {
        if (!this.currentUsername) return;

        this.isLoading = true;
        this.errorMessage = null;
        this.successMessage = null;

        this.balanceService
            .deposit(this.currentUsername, this.depositForm)
            .subscribe({
                next: () => {
                    this.successMessage = 'Средства успешно зачислены';
                    this.loadBalance();
                    this.loadTransactions();
                    this.activeTab = null;
                    this.depositForm = { amount: 0 };
                    this.isLoading = false;
                },
                error: (err) => {
                    this.errorMessage =
                        err.error?.message || 'Ошибка при пополнении баланса';
                    this.isLoading = false;
                },
            });
    }

    withdraw(): void {
        if (!this.currentUsername) return;

        this.isLoading = true;
        this.errorMessage = null;
        this.successMessage = null;

        this.balanceService
            .withdraw(this.currentUsername, this.withdrawForm)
            .subscribe({
                next: () => {
                    this.successMessage = 'Средства успешно сняты';
                    this.loadBalance();
                    this.loadTransactions();
                    this.activeTab = null;
                    this.withdrawForm = { amount: 0, confirmationCode: '' };
                    this.isLoading = false;
                },
                error: (err) => {
                    this.errorMessage =
                        err.error?.message || 'Ошибка при снятии средств';
                    this.isLoading = false;
                },
            });
    }

    getStatusClass(status: TransactionStatus): string {
        switch (status) {
            case TransactionStatus.COMPLETED:
                return 'completed';
            case TransactionStatus.FAILED:
                return 'failed';
            default:
                return 'pending';
        }
    }

    getOperationType(type: OperationType): string {
        return type === OperationType.DEPOSIT ? 'Пополнение' : 'Снятие';
    }

    clearMessages(): void {
        this.errorMessage = null;
        this.successMessage = null;
    }
}
