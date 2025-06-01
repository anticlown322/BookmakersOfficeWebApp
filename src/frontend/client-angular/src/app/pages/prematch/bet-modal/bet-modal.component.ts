import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
    selector: 'app-bet-modal',
    templateUrl: './bet-modal.component.html',
    styleUrls: ['./bet-modal.component.scss'],
    standalone: true,
    imports: [CommonModule, FormsModule],
})
export class BetModalComponent {
    @Input() betData!: {
        type: string;
        value?: string;
        matchId: string;
        lineType: string;
    };
    @Input() isLoading = false;

    @Output() confirmed = new EventEmitter<{ amount: number }>();
    @Output() canceled = new EventEmitter<void>();

    amount = 0;

    confirmBet(): void {
        if (this.amount > 0) {
            this.confirmed.emit({ amount: this.amount });
        }
    }
}
