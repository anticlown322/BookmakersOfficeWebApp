import { Component, Input } from '@angular/core';
import { Tournament } from '../../../core/models/sport-data-service/entities/tournament/tournament.model';
import { CommonModule } from '@angular/common';
import { MatchPrematchComponent } from '../match-prematch/match-prematch.component';

@Component({
  selector: 'app-tournament-prematch-card',
  templateUrl: './tournament-prematch.component.html',
  styleUrl: './tournament-prematch.component.scss',
  standalone: true,
  imports: [CommonModule, MatchPrematchComponent]
})
export class TournamentPrematchComponent {
  @Input() tournament!: Tournament;
  isExpanded = true;

  toggleExpansion(): void {
    this.isExpanded = !this.isExpanded;
  }
}