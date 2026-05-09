import { ChangeDetectionStrategy, Component, Input, Output, EventEmitter } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ProductResponse } from '../../../features/products/Core/Models/product-response.model';
import {
  PRODUCT_STATUS_MAP,
  STATUS_ENUM_MAP,
} from '../../../features/products/Core/Models/Configs/product-status.config';

@Component({
  selector: 'app-product-card',
  standalone: true,
  imports: [CommonModule, RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './product-card.html',
  styleUrl: './product-card.css',
})
export class ProductCard {
  @Input() product!: ProductResponse;

  @Input() deleting = false;

  @Output() delete = new EventEmitter<number>();

  onDelete(event: MouseEvent) {
    event.stopPropagation();
    event.preventDefault();
    if (!this.product || this.deleting) return;
    this.delete.emit(this.product.id);
  }

  get statusConfig() {
    const key = STATUS_ENUM_MAP[this.product.status as keyof typeof STATUS_ENUM_MAP];
    return (
      PRODUCT_STATUS_MAP[key] ?? {
        label: 'Unknown',
        color: 'bg-gray-200 text-gray-500',
      }
    );
  }
}
