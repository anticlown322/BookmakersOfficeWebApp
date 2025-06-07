import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InfiniteScrollDirective } from '../../shared/ui/infinite-scroll.directive';
import { TournamentResultComponent } from './tournament-result/tournament-result.component';
import { TournamentResult } from '../../core/models/sport-data-service/entities/tournament-result/tournament-result.model';
import { TournamentResultService } from '../../core/services/sport-data-service/tournament-result.service';
import { TournamentResultSignalrService } from '../../core/services/sport-data-service/tournament-result-signalr';

@Component({
    selector: 'app-results-page',
    templateUrl: './results.component.html',
    styleUrls: ['./results.component.scss'],
    standalone: true,
    imports: [CommonModule, TournamentResultComponent, InfiniteScrollDirective],
})
export class ResultsPageComponent implements OnInit {
    tournaments: TournamentResult[] = [];
    isLoading = true;
    isTransitioning = false;
    currentPage = 1;
    pageSize = 5;
    hasMore = true;
    MIN_LOADING_DISPLAY_TIME = 500;
    tournamentsCache: Record<number, TournamentResult[]> = {};

    constructor(
        private resultsSignalrService: TournamentResultSignalrService,
        private resultsService: TournamentResultService
    ) {}

    ngOnInit(): void {
        this.loadInitialTournaments();

        this.resultsSignalrService.startConnection();

        this.resultsSignalrService.resultsUpdated$.subscribe((update) => {
            if (update) {
                this.tournaments = update.tournaments;
                this.isLoading = false;
                this.isTransitioning = false;
                this.hasMore = update.tournaments.length === this.pageSize;
            }
        });
    }

    loadInitialTournaments(): void {
        const startTime = Date.now();
        this.isLoading = true;
        this.isTransitioning = true;
        this.tournaments = [];

        this.resultsService
            .getTournamentResults({
                pageNumber: this.currentPage,
                pageSize: this.pageSize,
            })
            .subscribe({
                next: (tournaments) => {
                    const elapsed = Date.now() - startTime;
                    const remainingTime = Math.max(
                        0,
                        this.MIN_LOADING_DISPLAY_TIME - elapsed
                    );

                    setTimeout(() => {
                        this.cacheData(this.currentPage, tournaments.items);
                        this.isLoading = false;
                        this.isTransitioning = false;
                        this.hasMore =
                            tournaments.items.length === this.pageSize;
                    }, remainingTime);
                },
                error: (error) => {
                    console.error('Error loading tournaments:', error);
                    this.isLoading = false;
                    this.isTransitioning = false;
                },
            });
    }

    loadMoreTournaments(): void {
        if (this.isTransitioning || !this.hasMore) return;

        const nextPage = this.currentPage + 1;
        const startTime = Date.now();
        this.isTransitioning = true;

        if (this.tournamentsCache[nextPage]) {
            this.applyCachedData(nextPage);
            return;
        }

        this.resultsService
            .getTournamentResults({
                pageNumber: nextPage,
                pageSize: this.pageSize,
            })
            .subscribe({
                next: (tournaments) => {
                    const elapsed = Date.now() - startTime;
                    const remainingTime = Math.max(
                        0,
                        this.MIN_LOADING_DISPLAY_TIME - elapsed
                    );

                    setTimeout(() => {
                        this.cacheData(nextPage, tournaments.items);
                        this.isTransitioning = false;
                        this.hasMore =
                            tournaments.items.length === this.pageSize;
                    }, remainingTime);
                },
                error: (error) => {
                    console.error('Error loading more tournaments:', error);
                    this.isTransitioning = false;
                },
            });
    }

    private cacheData(page: number, tournaments: TournamentResult[]): void {
        this.tournamentsCache[page] = tournaments;
        this.tournaments = [...this.tournaments, ...tournaments];
        this.currentPage = page;
    }

    private applyCachedData(page: number): void {
        const cachedTournaments = this.tournamentsCache[page];
        this.tournaments = [...this.tournaments, ...cachedTournaments];
        this.currentPage = page;
        this.isTransitioning = false;
    }

    trackByTournamentId(index: number, tournament: TournamentResult): string {
        return tournament.id;
    }

    ngOnDestroy(): void {
        this.resultsSignalrService.stopConnection();
    }
}
