import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  OnInit,
  computed,
  inject,
  signal,
} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import {
  catchError,
  debounceTime,
  distinctUntilChanged,
  finalize,
  map,
  switchMap,
  tap,
  of,
} from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Pagination } from '../../../../shared/components/pagination/pagination';
import { ProductCard } from '../../../../shared/components/product-card/product-card';
import { ProductService } from '../../Core/Service/product.service';
import { ProductResponse } from '../../Core/Models/product-response.model';
import { ErrorHandlerService } from '../../../../core/services/error-handler/error-handler.service';
import { ConfirmService } from '../../../../core/services/confirm/confirm.service';
import { AlertService } from '../../../../core/services/alert/alert';

interface ProductListQueryState {
  pageNumber: number;
  nameTerm: string | null;
  descriptionTerm: string | null;
  price: number | null;
  quantity: number | null;
}

@Component({
  selector: 'app-product-list',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule, Pagination, ProductCard],
  templateUrl: './product-list.html',
  styleUrl: './product-list.css',
})
export class ProductList implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly productService = inject(ProductService);
  private readonly errorHandler = inject(ErrorHandlerService);
  private readonly confirm = inject(ConfirmService);
  private readonly alert = inject(AlertService);
  private readonly destroyRef = inject(DestroyRef); // Used to manage unsubscription when the component is destroyed
  readonly deletingId = signal<number | null>(null);

  readonly pageSize = 9;
  readonly nameControl = new FormControl('', { nonNullable: true });
  readonly descriptionControl = new FormControl('', { nonNullable: true });
  readonly priceControl = new FormControl('', { nonNullable: true });
  readonly quantityControl = new FormControl('', { nonNullable: true });

  // State signals
  readonly loading = signal(true);
  readonly products = signal<ProductResponse[]>([]);
  readonly totalPages = signal(0);
  readonly currentPage = signal(1);
  readonly errorMessage = signal<string | null>(null);

  // Computed properties for template
  readonly hasProducts = computed(() => this.products().length > 0);
  readonly emptyMessage = computed(() => {
    const nameTerm = this.nameControl.value.trim();
    return nameTerm ? `No products found for “${nameTerm}”.` : 'No products found.';
  });

  ngOnInit(): void {
    this.bindQueryParams();
    this.bindSearch();
  }

  trackByProduct(_: number, product: ProductResponse): number {
    return product.id;
  }

  private bindQueryParams(): void {
    this.route.queryParamMap
      .pipe(
        map((params) => ({
          rawPageNumber: params.get('pageNumber'),
          rawNameTerm: params.get('name')?.trim() ?? '',
          rawDescriptionTerm: params.get('description')?.trim() ?? '',
          rawPrice: params.get('price'),
          rawQuantity: params.get('quantity'),
          state: this.normalizeQueryState(
            params.get('pageNumber'),
            params.get('name') ?? '',
            params.get('description') ?? '',
            params.get('price'),
            params.get('quantity'),
          ),
        })),
        // to avoid unnecessary reloads when query params change but the actual state is the same
        distinctUntilChanged(
          (previous, current) =>
            previous.state.pageNumber === current.state.pageNumber &&
            previous.state.nameTerm === current.state.nameTerm &&
            previous.state.descriptionTerm === current.state.descriptionTerm &&
            previous.state.price === current.state.price &&
            previous.state.quantity === current.state.quantity,
        ),
        // UI Update
        tap(({ rawPageNumber, rawNameTerm, rawDescriptionTerm, rawPrice, rawQuantity, state }) => {
          this.currentPage.set(state.pageNumber);

          const normalizedNameTerm = state.nameTerm ?? '';
          if (this.nameControl.value !== normalizedNameTerm) {
            this.nameControl.setValue(normalizedNameTerm, { emitEvent: false });
          }

          const normalizedDescription = state.descriptionTerm ?? '';
          if (this.descriptionControl.value !== normalizedDescription) {
            this.descriptionControl.setValue(normalizedDescription, { emitEvent: false });
          }

          const normalizedPriceInput = state.price === null ? '' : String(state.price);
          if (this.priceControl.value !== normalizedPriceInput) {
            this.priceControl.setValue(normalizedPriceInput, { emitEvent: false });
          }

          const normalizedQuantityInput = state.quantity === null ? '' : String(state.quantity);
          if (this.quantityControl.value !== normalizedQuantityInput) {
            this.quantityControl.setValue(normalizedQuantityInput, { emitEvent: false });
          }

          // If the raw query params differ from the normalized state, sync them back to the URL (this can happen if the user manually edits the URL with invalid values, for example)
          const normalizedPrice = state.price === null ? null : String(state.price);
          const normalizedQuantity = state.quantity === null ? null : String(state.quantity);

          if (
            rawPageNumber !== String(state.pageNumber) ||
            rawNameTerm !== state.nameTerm ||
            rawDescriptionTerm !== state.descriptionTerm ||
            rawPrice !== normalizedPrice ||
            rawQuantity !== normalizedQuantity
          ) {
            void this.syncQueryParams(state, true);
          }
        }),
        switchMap(({ state }) => this.loadProducts(state)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe();
  }

  private bindSearch(): void {
    this.nameControl.valueChanges
      .pipe(
        debounceTime(350),
        map((value) => value.trim()),
        distinctUntilChanged(),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((searchTerm) => {
        void this.syncQueryParams({
          pageNumber: 1,
          nameTerm: searchTerm,
          descriptionTerm: this.descriptionControl.value.trim(),
          price: this.parseOptionalNumber(this.priceControl.value),
          quantity: this.parseOptionalNumber(this.quantityControl.value),
        }); // Reset to page 1 on new search
      });

    this.descriptionControl.valueChanges
      .pipe(
        debounceTime(350),
        map((value) => value.trim()),
        distinctUntilChanged(),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((descriptionTerm) => {
        void this.syncQueryParams({
          pageNumber: 1,
          nameTerm: this.nameControl.value.trim(),
          descriptionTerm,
          price: this.parseOptionalNumber(this.priceControl.value),
          quantity: this.parseOptionalNumber(this.quantityControl.value),
        });
      });

    this.priceControl.valueChanges
      .pipe(
        debounceTime(350),
        map((value) => value.trim()),
        distinctUntilChanged(),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((priceInput) => {
        void this.syncQueryParams({
          pageNumber: 1,
          nameTerm: this.nameControl.value.trim(),
          descriptionTerm: this.descriptionControl.value.trim(),
          price: this.parseOptionalNumber(priceInput),
          quantity: this.parseOptionalNumber(this.quantityControl.value),
        });
      });

    this.quantityControl.valueChanges
      .pipe(
        debounceTime(350),
        map((value) => value.trim()),
        distinctUntilChanged(),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((quantityInput) => {
        void this.syncQueryParams({
          pageNumber: 1,
          nameTerm: this.nameControl.value.trim(),
          descriptionTerm: this.descriptionControl.value.trim(),
          price: this.parseOptionalNumber(this.priceControl.value),
          quantity: this.parseOptionalNumber(quantityInput),
        });
      });
  }

  onPageChange(pageNumber: number): void {
    void this.syncQueryParams({
      pageNumber,
      nameTerm: this.nameControl.value.trim(),
      descriptionTerm: this.descriptionControl.value.trim(),
      price: this.parseOptionalNumber(this.priceControl.value),
      quantity: this.parseOptionalNumber(this.quantityControl.value),
    });
  }

  private loadProducts(state: ProductListQueryState) {
    this.loading.set(true);
    this.errorMessage.set(null);

    return this.productService
      .getProducts({
        pageNumber: state.pageNumber,
        pageSize: this.pageSize,
        name: state.nameTerm,
        description: state.descriptionTerm,
        price: state.price,
        quantity: state.quantity,
      })
      .pipe(
        tap((response) => {
          const data = response.data;
          this.products.set(data?.items ?? []);
          this.totalPages.set(data?.totalPages ?? 0);
          this.currentPage.set(data?.pageNumber ?? state.pageNumber);
        }),
        catchError((error) => {
          this.products.set([]);
          this.totalPages.set(0);
          this.errorMessage.set(this.errorHandler.getMessage(error));
          return of(void 0);
        }),
        map(() => void 0),
        finalize(() => this.loading.set(false)),
      );
  }

  private normalizeQueryState(
    pageNumberValue: string | null,
    searchTermValue: string,
    descriptionTermValue: string,
    priceValue: string | null,
    quantityValue: string | null,
  ): ProductListQueryState {
    return {
      pageNumber: this.parsePageNumber(pageNumberValue),
      nameTerm: searchTermValue.trim(),
      descriptionTerm: descriptionTermValue.trim(),
      price: this.parseOptionalNumber(priceValue),
      quantity: this.parseOptionalNumber(quantityValue),
    };
  }

  async onDeleteProduct(productId: number): Promise<void> {
    const product = this.products().find((p) => p.id === productId);
    if (!product || this.deletingId()) return;

    const shouldDelete = await this.confirm.ask(
      `Are you sure you want to delete "${product.name}"? This action can be undone only by support.`,
      'Delete Product',
    );

    if (!shouldDelete) return;

    this.deletingId.set(productId);

    this.productService
      .deleteProduct(productId)
      .pipe(
        finalize(() => this.deletingId.set(null)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: () => {
          this.alert.success('Product deleted successfully.');
          void this.loadProducts({
            pageNumber: this.currentPage(),
            nameTerm: this.nameControl.value.trim(),
            descriptionTerm: this.descriptionControl.value.trim(),
            price: this.parseOptionalNumber(this.priceControl.value),
            quantity: this.parseOptionalNumber(this.quantityControl.value),
          }).subscribe();
        },
      });
  }

  // Parses the page number from the query param, ensuring it's a valid positive integer. Defaults to 1 if invalid.
  private parsePageNumber(pageNumberValue: string | null): number {
    const parsedPageNumber = Number(pageNumberValue);
    return Number.isInteger(parsedPageNumber) && parsedPageNumber > 0 ? parsedPageNumber : 1;
  }

  private parseOptionalNumber(value: string | null): number | null {
    if (value === null) {
      return null;
    }

    const trimmed = value.trim();
    if (!trimmed) {
      return null;
    }

    const parsed = Number(trimmed);
    return Number.isFinite(parsed) ? parsed : null;
  }

  private async syncQueryParams(state: ProductListQueryState, replaceUrl = false): Promise<void> {
    await this.router.navigate([], {
      relativeTo: this.route,
      queryParams: {
        pageNumber: state.pageNumber,
        name: state.nameTerm || null,
        description: state.descriptionTerm || null,
        price: state.price,
        quantity: state.quantity,
      },
      replaceUrl,
    });
  }
}
