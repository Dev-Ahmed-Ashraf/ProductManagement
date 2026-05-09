import { Component, Input, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-stats-card',
  standalone: true,
  imports: [CommonModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './stats-card.html',
  styleUrl: './stats-card.css',
})
export class StatsCard {
  @Input() title = '';
  @Input() value: number | string | null = null;
  @Input() subtitle: string | null = null;
  @Input() loading = false;
  @Input() tone: 'teal' | 'blue' | 'amber' | 'rose' | 'slate' | 'violet' = 'teal';
}
