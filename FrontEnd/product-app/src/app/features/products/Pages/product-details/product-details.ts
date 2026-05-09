import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  OnInit,
  computed,
  inject,
  signal,
} from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { catchError, finalize, map, of, switchMap, tap } from 'rxjs';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { ProductService } from '../../Core/Service/product.service';
import { ErrorHandlerService } from '../../../../core/services/error-handler/error-handler.service';
import { PRODUCT_DETAIL_ACTIONS, filterProductActions } from '../../products.features';
import { AuthService } from '../../../../core/services/auth/auth';
import { ConfirmService } from '../../../../core/services/confirm/confirm.service';
import { AlertService } from '../../../../core/services/alert/alert';
import { Router } from '@angular/router';
import { StatusDropdown } from '../../../../shared/components/status-dropdown/status-dropdown';
import {
  PRODUCT_STATUS_MAP,
  STATUS_ENUM_MAP,
} from '../../Core/Models/Configs/product-status.config';
import { StatusBadge } from '../../../../shared/components/status-badge/status-badge';
import { ProductWhistoriesResponse } from '../../Core/Models/productWhistoriesResponse.model';
import { ProductStatusHistoriesResponse } from '../../Core/Models/ProductStatusHistoriesResponse.model';

@Component({
  selector: 'app-product-details',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, RouterLink, StatusDropdown, StatusBadge],
  templateUrl: './product-details.html',
  styleUrl: './product-details.css',
})
export class ProductDetails implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly productService = inject(ProductService);
  private readonly errorHandler = inject(ErrorHandlerService);
  private readonly confirm = inject(ConfirmService);
  private readonly alert = inject(AlertService);
  private readonly auth = inject(AuthService);
  private readonly destroyRef = inject(DestroyRef);

  private readonly permissions = toSignal(this.auth.permissions$, {
    initialValue: new Set<string>(),
  });

  readonly loading = signal(true);
  readonly updatingStatus = signal(false);
  readonly deleting = signal(false);
  readonly product = signal<ProductWhistoriesResponse | null>(null);
  readonly errorMessage = signal<string | null>(null);

  readonly visibleActions = computed(() =>
    filterProductActions(this.permissions(), PRODUCT_DETAIL_ACTIONS),
  );

  readonly histories = signal<ProductStatusHistoriesResponse[]>([]);
  readonly historiesLoading = signal(false);
  readonly historiesPage = signal(1);
  readonly historiesPageSize = signal(5);
  readonly historiesTotalPages = signal(0);

  readonly hasProduct = computed(() => !!this.product());

  actionIconPath(icon: string): string {
    switch (icon) {
      case 'trash':
        return 'M3 6h18M8 6V4h8v2M6 6l1 14h10l1-14M10 11v5M14 11v5';
      default:
        return 'M9 12h6M12 9v6M4 12a8 8 0 1 0 16 0 8 8 0 0 0-16 0Z';
    }
  }

  ngOnInit(): void {
    this.route.paramMap
      .pipe(
        map((params) => Number(params.get('id'))),
        switchMap((id) => this.loadProduct(id)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe();
  }

  async onDeleteProduct(): Promise<void> {
    const currentProduct = this.product();

    if (!currentProduct || this.deleting()) {
      return;
    }

    const shouldDelete = await this.confirm.ask(
      `Are you sure you want to delete "${currentProduct.name}"? This action can be undone only by support.`,
      'Delete Product',
    );

    if (!shouldDelete) {
      return;
    }

    this.deleting.set(true);

    this.productService
      .deleteProduct(currentProduct.id)
      .pipe(
        finalize(() => this.deleting.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: () => {
          this.alert.success('Product deleted successfully.');
          this.product.set(null);
          void this.router.navigate(['/dashboard/products']);
        },
      });
  }

  async onStatusChange(newStatus: number): Promise<void> {
    const product = this.product();
    if (!product || this.updatingStatus()) {
      return;
    }

    if (product.status === newStatus) {
      return;
    }

    const shouldUpdate = await this.confirm.ask(
      `Change status from "${this.resolveStatusLabel(product.status)}" to "${this.resolveStatusLabel(newStatus)}"?`,
      'Change Status',
    );

    if (!shouldUpdate) {
      return;
    }

    this.updatingStatus.set(true);

    this.productService
      .changeStatus(product.id, newStatus)
      .pipe(
        finalize(() => this.updatingStatus.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (res) => {
          const updatedStatus = res.data?.newStatus;
          if (updatedStatus === undefined) return;

          this.product.set({
            ...product,
            status: updatedStatus,
          });

          this.alert.success(`Status updated to "${this.resolveStatusLabel(updatedStatus)}".`);

          void this.loadProduct(res.data?.productId ?? product.id)
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe();
        },
      });
  }

  private loadProduct(id: number) {
    if (!Number.isInteger(id) || id < 1) {
      this.errorMessage.set('Invalid product identifier.');
      this.product.set(null);
      this.loading.set(false);
      return of(void 0);
    }

    this.loading.set(true);
    this.errorMessage.set(null);

    return this.productService.getProductById(id).pipe(
      tap((response) => {
        this.product.set(response.data ?? null);
        this.histories.set(response.data?.history ?? []);
      }),
      catchError((error) => {
        this.product.set(null);
        this.errorMessage.set(this.errorHandler.getMessage(error));
        return of(void 0);
      }),
      map(() => void 0),
      finalize(() => this.loading.set(false)),
    );
  }

  onHistoriesPageChange(page: number): void {
    const product = this.product();
    if (!product) return;
    this.historiesPage.set(page);
    void this.loadProduct(product.id).pipe(takeUntilDestroyed(this.destroyRef)).subscribe();
  }

  // This method is used in the template to display the status label based on the status value.
  resolveStatusLabel(status: number): string {
    const key = STATUS_ENUM_MAP[status as keyof typeof STATUS_ENUM_MAP];

    return PRODUCT_STATUS_MAP[key]?.label ?? 'Unknown';
  }

}
