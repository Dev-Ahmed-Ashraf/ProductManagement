import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import {
  PRODUCT_STATUS_MAP,
  STATUS_ENUM_MAP,
} from '../../../features/products/Core/Models/Configs/product-status.config';

@Component({
  selector: 'app-status-badge',
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './status-badge.html',
  styleUrl: './status-badge.css',
})
export class StatusBadge {
  @Input() status: number | undefined;

  get config() {
    const key = STATUS_ENUM_MAP[this.status as keyof typeof STATUS_ENUM_MAP];
    return (
      PRODUCT_STATUS_MAP[key] ?? {
        label: 'Unknown',
        color: 'bg-gray-200 text-gray-500',
      }
    );
  }
}
