import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { Team } from '../../core/models/sport-data-service/entities/team/team.model';
import { TeamService } from '../../core/services/sport-data-service/team.service';

@Component({
    selector: 'app-teams',
    templateUrl: './teams.component.html',
    styleUrls: ['./teams.component.scss'],
    standalone: true,
    imports: [CommonModule, RouterModule],
})
export class TeamsComponent implements OnInit {
    teams: Team[] = [];
    isLoading = true;
    currentPage = 1;
    pageSize = 10;
    totalPages = 0;
    totalCount = 0;
    hasPrevious = false;
    hasNext = false;
    isTransitioning = false;
    MIN_LOADING_DISPLAY_TIME = 500;

    teamsCache: Record<number, { items: Team[]; metaData: any }> = {};

    constructor(private teamService: TeamService) {}

    ngOnInit(): void {
        this.loadInitialTeams();
    }

    loadInitialTeams(): void {
        const startTime = Date.now();

        this.isLoading = true;
        this.isTransitioning = true;
        this.teams = []; 

        this.teamService
            .getTeams({ pageNumber: this.currentPage, pageSize: this.pageSize })
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
                    console.error('Error loading teams:', error);
                    this.isLoading = false;
                    this.isTransitioning = false;
                },
            });
    }

    loadTeams(page: number): void {
        if (this.isTransitioning || page === this.currentPage) return;

        const startTime = Date.now();
        this.isTransitioning = true;
        this.teams = [];

        if (this.teamsCache[page]) {
                this.applyCachedData(page);
            return;
        }

        this.teamService
            .getTeams({ pageNumber: page, pageSize: this.pageSize })
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
                    console.error('Error loading teams:', error);
                    this.isTransitioning = false;
                },
            });
    }

    private applyCachedData(page: number): void {
        const cached = this.teamsCache[page];
        this.teams = cached.items;
        this.currentPage = page;
        this.totalPages = cached.metaData.TotalPages;
        this.totalCount = cached.metaData.TotalCount;
        this.hasPrevious = cached.metaData.HasPrevious;
        this.hasNext = cached.metaData.HasNext;
        this.isLoading = false;
        setTimeout(() => {
            this.isTransitioning = false;
        }, 50);
    }

    private cacheData(page: number, items: Team[], metaData: any): void {
        this.teamsCache[page] = { items, metaData };
        this.applyCachedData(page);
    }

    trackByTeamId(index: number, team: Team): string {
        return team.id;
    }

    onPageChange(page: number): void {
        if (page < 1 || page > this.totalPages) return;
        this.loadTeams(page);
    }
}
