import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'app-unauthorized',
  standalone: true,
  templateUrl: './unauthorized.html',
  styleUrls: ['./unauthorized.css'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Unauthorized {}
