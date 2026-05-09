import {
  Component,
  OnInit,
  inject,
  DestroyRef,
  ChangeDetectionStrategy,
  signal,
  computed,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import {
  debounceTime,
  distinctUntilChanged,
  map,
  switchMap,
  tap,
  finalize,
} from 'rxjs';
import { takeUntilDestroyed as takeUntilDestroyedInterop } from '@angular/core/rxjs-interop';
import { UserService } from '../../core/services/user-service';
import { UserResponse } from '../../core/models/user-response.model';
import { PagedResponse } from '../../../../core/Models/paged-response.model';
import { ApiResponse } from '../../../../core/Models/GenericResponse.model';
import { Pagination } from '../../../../shared/components/pagination/pagination';
import { RouterLink } from '@angular/router';

interface UsersListQueryState {
  pageNumber: number;
  name: string | null;
  userName: string | null;
  email: string | null;
  role: string | null;
  isActive: boolean | null;
}

@Component({
  selector: 'app-users-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, Pagination, RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './users-list.html',
  styleUrl: './users-list.css',
})
export class UsersList implements OnInit {
  private readonly userService = inject(UserService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  readonly pageSize = 10;

  // Form Controls
  readonly nameControl = new FormControl('', { nonNullable: true });
  readonly userNameControl = new FormControl('', { nonNullable: true });
  readonly emailControl = new FormControl('', { nonNullable: true });
  readonly roleControl = new FormControl('', { nonNullable: true });
  readonly isActiveControl = new FormControl<'' | 'true' | 'false'>('', {
    nonNullable: true,
  });

  // State signals
  readonly loading = signal(false);
  readonly users = signal<UserResponse[]>([]);
  readonly totalPages = signal(0);
  readonly currentPage = signal(1);
  readonly errorMessage = signal<string | null>(null);

  // Computed properties
  readonly hasUsers = computed(() => this.users().length > 0);
  readonly emptyMessage = computed(() => {
    const name = this.nameControl.value.trim();
    return name ? `No users found for "${name}".` : 'No users found.';
  });

  ngOnInit(): void {
    this.bindQueryParams();
    this.bindSearch();
  }

  trackByUser(_: number, user: UserResponse): string {
    return user.id.toString();
  }

  onPageChange(pageNumber: number): void {
    void this.syncQueryParams({
      pageNumber,
      name: this.nameControl.value.trim(),
      userName: this.userNameControl.value.trim(),
      email: this.emailControl.value.trim(),
      role: this.roleControl.value.trim(),
      isActive: this.parseIsActive(this.isActiveControl.value),
    });
  }

  private bindQueryParams(): void {
    this.route.queryParamMap
      .pipe(
        map((params) => {
          const state = this.normalizeQueryState(
            params.get('pageNumber'),
            params.get('name') ?? '',
            params.get('userName') ?? '',
            params.get('email') ?? '',
            params.get('role') ?? '',
            params.get('isActive'),
          );
          return { state };
        }),
        distinctUntilChanged(
          (prev, curr) =>
            prev.state.pageNumber === curr.state.pageNumber &&
            prev.state.name === curr.state.name &&
            prev.state.userName === curr.state.userName &&
            prev.state.email === curr.state.email &&
            prev.state.role === curr.state.role &&
            prev.state.isActive === curr.state.isActive,
        ),
        tap(({ state }) => {
          this.currentPage.set(state.pageNumber);
          if (this.nameControl.value !== (state.name ?? '')) {
            this.nameControl.setValue(state.name ?? '', { emitEvent: false });
          }
          if (this.userNameControl.value !== (state.userName ?? '')) {
            this.userNameControl.setValue(state.userName ?? '', { emitEvent: false });
          }
          if (this.emailControl.value !== (state.email ?? '')) {
            this.emailControl.setValue(state.email ?? '', { emitEvent: false });
          }
          if (this.roleControl.value !== (state.role ?? '')) {
            this.roleControl.setValue(state.role ?? '', { emitEvent: false });
          }
          const isActiveValue = state.isActive === null ? '' : String(state.isActive);
          if (this.isActiveControl.value !== isActiveValue) {
            this.isActiveControl.setValue(isActiveValue as '' | 'true' | 'false', {
              emitEvent: false,
            });
          }
        }),
        switchMap(({ state }) => this.loadUsers(state)),
        takeUntilDestroyedInterop(this.destroyRef),
      )
      .subscribe();
  }

  private bindSearch(): void {
    this.nameControl.valueChanges
      .pipe(
        debounceTime(350),
        map((v) => v.trim()),
        distinctUntilChanged(),
        takeUntilDestroyedInterop(this.destroyRef),
      )
      .subscribe(() => this.applyFilters());

    this.userNameControl.valueChanges
      .pipe(
        debounceTime(350),
        map((v) => v.trim()),
        distinctUntilChanged(),
        takeUntilDestroyedInterop(this.destroyRef),
      )
      .subscribe(() => this.applyFilters());

    this.emailControl.valueChanges
      .pipe(
        debounceTime(350),
        map((v) => v.trim()),
        distinctUntilChanged(),
        takeUntilDestroyedInterop(this.destroyRef),
      )
      .subscribe(() => this.applyFilters());

    this.roleControl.valueChanges
      .pipe(
        debounceTime(350),
        map((v) => v.trim()),
        distinctUntilChanged(),
        takeUntilDestroyedInterop(this.destroyRef),
      )
      .subscribe(() => this.applyFilters());

    this.isActiveControl.valueChanges
      .pipe(distinctUntilChanged(), takeUntilDestroyedInterop(this.destroyRef))
      .subscribe(() => this.applyFilters());
  }

  private applyFilters(): void {
    void this.syncQueryParams({
      pageNumber: 1,
      name: this.nameControl.value.trim(),
      userName: this.userNameControl.value.trim(),
      email: this.emailControl.value.trim(),
      role: this.roleControl.value.trim(),
      isActive: this.parseIsActive(this.isActiveControl.value),
    });
  }

  private loadUsers(state: UsersListQueryState) {
    this.loading.set(true);
    this.errorMessage.set(null);

    return this.userService
      .getUsers({
        pageNumber: state.pageNumber,
        pageSize: this.pageSize,
        name: state.name,
        userName: state.userName,
        email: state.email,
        role: state.role,
        isActive: state.isActive,
      })
      .pipe(
        tap((response: ApiResponse<PagedResponse<UserResponse>>) => {
          const data = response.data;
          this.users.set(data?.items ?? []);
          this.totalPages.set(data?.totalPages ?? 0);
          this.currentPage.set(data?.pageNumber ?? state.pageNumber);
        }),
        map(() => void 0),
        finalize(() => this.loading.set(false)),
      );
  }

  // Utility methods for query param normalization and parsing
  private normalizeQueryState(
    pageNumberValue: string | null,
    nameValue: string,
    userNameValue: string,
    emailValue: string,
    roleValue: string,
    isActiveValue: string | null,
  ): UsersListQueryState {
    return {
      pageNumber: this.parsePageNumber(pageNumberValue),
      name: nameValue.trim() || null,
      userName: userNameValue.trim() || null,
      email: emailValue.trim() || null,
      role: roleValue.trim() || null,
      isActive: this.parseIsActive(isActiveValue),
    };
  }

  // Parses page number from query param, defaults to 1 if invalid
  private parsePageNumber(value: string | null): number {
    const parsed = Number(value);
    return Number.isInteger(parsed) && parsed > 0 ? parsed : 1;
  }

  private parseIsActive(value: string | null): boolean | null {
    if (value === 'true') return true;
    if (value === 'false') return false;
    return null;
  }

  private async syncQueryParams(state: UsersListQueryState, replaceUrl = false): Promise<void> {
    await this.router.navigate([], {
      relativeTo: this.route,
      queryParams: {
        pageNumber: state.pageNumber,
        name: state.name || null,
        userName: state.userName || null,
        email: state.email || null,
        role: state.role || null,
        isActive: state.isActive,
      },
      replaceUrl,
    });
  }
}
