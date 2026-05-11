import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  OnInit,
  computed,
  inject,
  signal,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { debounceTime, distinctUntilChanged, finalize } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Pagination } from '../../../../../shared/components/pagination/pagination';
import { StatusBadge } from '../../../../../shared/components/status-badge/status-badge';
import { ErrorHandlerService } from '../../../../../core/services/error-handler/error-handler.service';
import { StatusHistoriesService } from '../../../Core/Services/status-histories.service';
import { ProductStatusHistory } from '../../../Core/Models/product-status-histories.model';
import { ProductStatusHistoriesFilters } from '../../../Core/Models/ProductStatusHistoriesFilters.model';
import {
  PRODUCT_STATUS_MAP,
  STATUS_ENUM_MAP,
} from '../../../../products/Core/Models/Configs/product-status.config';

interface StatusHistoriesQueryState {
  pageNumber: number;
  productId: number | null;
  oldStatus: number | null;
  newStatus: number | null;
  fromDate: Date | null;
  toDate: Date | null;
}

@Component({
  selector: 'app-status-histories-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, ReactiveFormsModule, Pagination, RouterLink, StatusBadge],
  templateUrl: './status-histories-list.html',
  styleUrl: './status-histories-list.css',
})
export class StatusHistoriesList implements OnInit {
  private readonly historiesService = inject(StatusHistoriesService);
  private readonly errorHandler = inject(ErrorHandlerService);
  private readonly destroyRef = inject(DestroyRef);

  readonly pageSize = 8;

  readonly productIdControl = new FormControl('', { nonNullable: true });
  readonly oldStatusControl = new FormControl('', { nonNullable: true });
  readonly newStatusControl = new FormControl('', { nonNullable: true });
  readonly fromDateControl = new FormControl('', { nonNullable: true });
  readonly toDateControl = new FormControl('', { nonNullable: true });

  readonly loading = signal(true);
  readonly histories = signal<ProductStatusHistory[]>([]);
  readonly totalPages = signal(0);
  readonly totalItems = signal(0);
  readonly currentPage = signal(1);
  readonly errorMessage = signal<string | null>(null);

  readonly hasHistories = computed(() => this.histories().length > 0);
  readonly hasActiveFilters = computed(() => {
    return !!(
      this.productIdControl.value.trim() ||
      this.oldStatusControl.value.trim() ||
      this.newStatusControl.value.trim() ||
      this.fromDateControl.value.trim() ||
      this.toDateControl.value.trim()
    );
  });

  readonly statusOptions = [
    { value: '', label: 'All statuses' },
    ...Object.entries(STATUS_ENUM_MAP).map(([value, key]) => ({
      value,
      label: PRODUCT_STATUS_MAP[key].label,
    })),
  ];

  ngOnInit(): void {
    this.bindFilters();
    this.loadHistories(1);
  }

  trackById(_: number, history: ProductStatusHistory): number {
    return history.id;
  }

  onPageChange(pageNumber: number): void {
    this.loadHistories(pageNumber);
  }

  clearFilters(): void {
    this.productIdControl.setValue('', { emitEvent: false });
    this.oldStatusControl.setValue('', { emitEvent: false });
    this.newStatusControl.setValue('', { emitEvent: false });
    this.fromDateControl.setValue('', { emitEvent: false });
    this.toDateControl.setValue('', { emitEvent: false });
    this.loadHistories(1);
  }

  resolveStatusLabel(status: number | undefined): string {
    if (status === undefined || status === null) {
      return 'Unknown';
    }

    const key = STATUS_ENUM_MAP[status as keyof typeof STATUS_ENUM_MAP];
    return PRODUCT_STATUS_MAP[key]?.label ?? 'Unknown';
  }

  private bindFilters(): void {
    const reloadFromFirstPage = () => this.loadHistories(1);

    this.productIdControl.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe(reloadFromFirstPage);

    this.oldStatusControl.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe(reloadFromFirstPage);

    this.newStatusControl.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe(reloadFromFirstPage);

    this.fromDateControl.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe(reloadFromFirstPage);

    this.toDateControl.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe(reloadFromFirstPage);
  }

  private loadHistories(pageNumber: number): void {
    this.loading.set(true);
    this.errorMessage.set(null);

    const filters = this.buildFilters(pageNumber);

    this.historiesService
      .getStatusHistoriesByProductId(filters)
      .pipe(
        finalize(() => this.loading.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (response) => {
          const data = response.data;
          this.histories.set(data?.items ?? []);
          this.totalPages.set(data?.totalPages ?? 0);
          this.totalItems.set(data?.totalCount ?? 0);
          this.currentPage.set(data?.pageNumber ?? pageNumber);
        },
        error: (error: unknown) => {
          this.histories.set([]);
          this.totalPages.set(0);
          this.totalItems.set(0);
          this.errorMessage.set(this.errorHandler.getMessage(error));
        },
      });
  }

  private buildFilters(pageNumber: number): ProductStatusHistoriesFilters {
    return {
      pageNumber,
      pageSize: this.pageSize,
      productId: this.parseOptionalNumber(this.productIdControl.value),
      oldStatus: this.parseOptionalNumber(this.oldStatusControl.value),
      newStatus: this.parseOptionalNumber(this.newStatusControl.value),
      fromDate: this.parseOptionalDate(this.fromDateControl.value),
      toDate: this.parseOptionalDate(this.toDateControl.value),
    };
  }

  private parseOptionalNumber(value: string): number | undefined {
    const trimmed = value.trim();
    if (!trimmed) {
      return undefined;
    }

    const parsed = Number(trimmed);
    return Number.isFinite(parsed) ? parsed : undefined;
  }

  private parseOptionalDate(value: string): Date | undefined {
    const trimmed = value.trim();
    if (!trimmed) {
      return undefined;
    }

    const parsed = new Date(`${trimmed}T00:00:00`);
    return Number.isNaN(parsed.getTime()) ? undefined : parsed;
  }
}
