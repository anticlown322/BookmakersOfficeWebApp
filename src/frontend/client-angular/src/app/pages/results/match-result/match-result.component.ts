import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatchResult } from '../../../core/models/sport-data-service/entities/match-result/match-result.model';
import { ResultStatus } from '../../../core/models/sport-data-service/entities/match-result/result-status.enum';
import { ResultStatusPipe } from "../../../shared/ui/result-status.pipe";
import { CapitalizePipe } from "../../../shared/ui/capitalize.pipe";

@Component({
  selector: 'app-match-result',
  templateUrl: './match-result.component.html',
  styleUrls: ['./match-result.component.scss'],
  standalone: true,
  imports: [CommonModule, ResultStatusPipe, CapitalizePipe]
})
export class MatchResultComponent {
  @Input() match!: MatchResult;
  isDetailsExpanded = true;

  toggleDetails(): void {
    this.isDetailsExpanded = !this.isDetailsExpanded;
  }

  getMatchStatusClass(): string {
    switch(this.match.status) {
      case ResultStatus.Ended: return 'ended';
      case ResultStatus.Running: return 'running';
      case ResultStatus.Interrupted: return 'interrupted';
      case ResultStatus.Canceled: return 'canceled';
      default: return '';
    }
  }
}