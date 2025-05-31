import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TournamentService } from '../../core/services/sport-data-service/tournament.service';
import { TournamentPrematchComponent } from './tournament-prematch/tournament-prematch.component';
import { InfiniteScrollDirective } from '../../shared/ui/infinite-scroll.directive';
import { Tournament } from '../../core/models/sport-data-service/entities/tournament/tournament.model';
import { AuthService } from '../../core/services/user-service/auth.service';
import { TournamentSignalrService } from '../../core/services/sport-data-service/tournament-signalr';

@Component({
    selector: 'app-prematch',
    templateUrl: './prematch.component.html',
    styleUrls: ['./prematch.component.scss'],
    standalone: true,
    imports: [
        CommonModule,
        TournamentPrematchComponent,
        InfiniteScrollDirective,
    ],
})
export class PrematchComponent implements OnInit {
    tournaments: Tournament[] = [];
    isLoading = true;
    isTransitioning = false;
    currentPage = 1;
    pageSize = 5;
    hasMore = true;
    MIN_LOADING_DISPLAY_TIME = 500;

    constructor(
        private tournamentSignalrService: TournamentSignalrService,
        private tournamentService: TournamentService,
        authService: AuthService
    ) {}

    ngOnInit(): void {
        this.loadInitialTournaments();

        this.tournamentSignalrService.startConnection();

        this.tournamentSignalrService.prematchUpdated$.subscribe((update) => {
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

        this.tournamentService
            .getTournaments({
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
                        this.tournaments = tournaments.items;
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

        this.tournamentService
            .getTournaments({
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
                        this.tournaments = [
                            ...this.tournaments,
                            ...tournaments.items,
                        ];
                        this.currentPage = nextPage;
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

    trackByTournamentId(index: number, tournament: Tournament): string {
        return tournament.id;
    }

    ngOnDestroy(): void {
        this.tournamentSignalrService.stopConnection();
    }
}
