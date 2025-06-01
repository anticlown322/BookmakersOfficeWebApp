import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TournamentResult } from '../../../core/models/sport-data-service/entities/tournament-result/tournament-result.model';
import { ResultStatus } from '../../../core/models/sport-data-service/entities/match-result/result-status.enum';
import { MatchResultComponent } from '../match-result/match-result.component';

@Component({
  selector: 'app-tournament-result-card',
  templateUrl: './tournament-result.component.html',
  styleUrls: ['./tournament-result.component.scss'],
  standalone: true,
  imports: [CommonModule, MatchResultComponent]
})
export class TournamentResultComponent {
  @Input() tournament!: TournamentResult;
  isExpanded = true;

  toggleExpansion(): void {
    this.isExpanded = !this.isExpanded;
  }

  getStatusClass(status?: ResultStatus): string {
    switch(status) {
      case ResultStatus.Ended: return 'ended';
      case ResultStatus.Running: return 'running';
      case ResultStatus.Interrupted: return 'interrupted';
      case ResultStatus.Canceled: return 'canceled';
      default: return '';
    }
  }
}