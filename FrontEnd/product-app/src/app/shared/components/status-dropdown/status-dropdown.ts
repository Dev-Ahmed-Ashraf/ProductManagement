import { Component, EventEmitter, Input, Output } from '@angular/core';
import {
  PRODUCT_STATUS_MAP,
  STATUS_ENUM_MAP,
  type StatusConfig,
} from '../../../features/products/Core/Models/Configs/product-status.config';
import { StatusBadge } from '../status-badge/status-badge';

interface StatusOption {
  value: number;
  label: string;
}

const FALLBACK_STATUS: StatusConfig = {
  label: 'Unknown',
  color: 'bg-slate-100 text-slate-600',
};

@Component({
  selector: 'app-status-dropdown',
  imports: [StatusBadge],
  templateUrl: './status-dropdown.html',
  styleUrl: './status-dropdown.css',
})
export class StatusDropdown {
  @Input() currentStatus!: number;
  @Input() disabled = false;
  @Output() statusChange = new EventEmitter<number>();

  readonly statuses: StatusOption[] = Object.entries(STATUS_ENUM_MAP).map(([value, key]) => ({
    value: Number(value),
    label: PRODUCT_STATUS_MAP[key].label,
  }));

  get currentStatusConfig(): StatusConfig {
    const key = STATUS_ENUM_MAP[this.currentStatus as keyof typeof STATUS_ENUM_MAP];
    return PRODUCT_STATUS_MAP[key] ?? FALLBACK_STATUS;
  }

  onChange(newStatus: string) {
    const nextStatus = Number(newStatus);

    if (this.currentStatus === nextStatus) {
      return;
    }

    this.statusChange.emit(nextStatus);
  }
}
