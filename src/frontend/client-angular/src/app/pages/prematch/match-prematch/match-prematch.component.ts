import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Match } from '../../../core/models/sport-data-service/entities/match/match.model';
import { BetsService } from '../../../core/services/betting-service/bets.service';
import { BetModalComponent } from '../bet-modal/bet-modal.component';
import { MarketValue } from '../../../core/models/sport-data-service/entities/match/market-value.model';
import { PlaceBetRequest } from '../../../core/models/betting-serivce/requests/bet/place-bet.request';

@Component({
    selector: 'app-match-prematch',
    templateUrl: './match-prematch.component.html',
    styleUrls: ['./match-prematch.component.scss'],
    standalone: true,
    imports: [CommonModule, BetModalComponent],
})
export class MatchPrematchComponent {
    @Input() match!: Match;
    activeBet: {
        type: string;
        marketValue: string;
        lineType: string;
        marketSelection: string;
    } | null = null;
    isDetailsExpanded = true;

    constructor(private betsService: BetsService) {}

    toggleDetails(): void {
        this.isDetailsExpanded = !this.isDetailsExpanded;
    }

    placeBet(type: string, market?: MarketValue, lineType?: string): void {
        this.activeBet = {
            type,
            marketValue: market?.value || '1',
            lineType: lineType || this.detectLineType(type),
            marketSelection: this.convertToMarketSelection(type),
        };
    }

    onBetConfirmed(amount: number): void {
        if (!this.activeBet) return;

        console.log(this.match.id);
        console.log(this.activeBet?.lineType);
        console.log(this.activeBet?.marketSelection);

        const betData: PlaceBetRequest = {
            matchId: this.match.id,
            amount: amount,
            lineType: this.activeBet.lineType,
            marketSelection: this.activeBet.marketSelection,
            odds: this.activeBet.marketValue,
        };

        this.betsService.placeBet(betData).subscribe({
            next: () => {
                console.log('Ставка успешно размещена');
                this.activeBet = null;
            },
            error: (err) => {
                console.error('Ошибка при размещении ставки:', err);
                this.activeBet = null;
            },
        });
    }

    private detectLineType(type: string): string {
        if (type.includes('map')) return 'MapsLine';
        if (type.includes('Kills')) return 'KillsLine';
        if (type.includes('Special')) return 'SpecialLine';
        return 'MainLine';
    }

    private convertToMarketSelection(type: string): string {
        return type.charAt(0).toUpperCase() + type.slice(1);
    }

    hasMainLine(): boolean {
        return !!this.match.mainLine;
    }

    hasMapsLine(): boolean {
        return !!this.match.mapsLine;
    }

    hasKillsLine(): boolean {
        return !!this.match.killsLine;
    }

    hasSpecialLine(): boolean {
        return !!this.match.specialLine;
    }
}
