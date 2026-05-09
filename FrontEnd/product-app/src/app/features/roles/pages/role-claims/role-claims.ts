import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  OnInit,
  computed,
  inject,
  signal,
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router } from '@angular/router';
import { RoleClaimsResponse } from '../../core/models/role-claims-response.model';
import { RoleResponse } from '../../core/models/role-response.model';
import { RoleService } from '../../core/service/role-service';
import { RoleDropdown } from '../../../../shared/components/role-dropdown/role-dropdown';

@Component({
  selector: 'app-role-claims',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RoleDropdown],
  templateUrl: './role-claims.html',
  styleUrl: './role-claims.css',
})
export class RoleClaims implements OnInit {
  private readonly roleService = inject(RoleService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  readonly roles = signal<RoleResponse[]>([]);
  readonly selectedRoleId = signal<string | null>(null);
  readonly claims = signal<RoleClaimsResponse[]>([]);
  readonly loadingRoles = signal(false);
  readonly loadingClaims = signal(false);
  readonly errorMessage = signal<string | null>(null);

  readonly hasClaims = computed(() => this.claims().length > 0);

  ngOnInit(): void {
    this.loadRoles();

    this.route.queryParamMap.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((params) => {
      const raw = params.get('roleId');
      const parsed = raw ? raw : null;
      this.selectedRoleId.set(parsed);

      const roleId = this.selectedRoleId();
      if (roleId === null) {
        this.claims.set([]);
        return;
      }

      this.loadRoleClaims(roleId);
    });
  }

  onRoleChange(roleId: string | null): void {
    void this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { roleId },
      queryParamsHandling: 'merge',
    });
  }

  trackByRole(_: number, role: RoleResponse): number {
    return role.id;
  }

  trackByClaim(_: number, claim: RoleClaimsResponse): string {
    return `${claim.type}:${claim.value}`;
  }

  private loadRoles(): void {
    this.loadingRoles.set(true);
    this.errorMessage.set(null);

    this.roleService
      .getRoles()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (response) => {
          this.roles.set(response.data ?? []);
          this.loadingRoles.set(false);
        },
        error: () => {
          this.errorMessage.set('Failed to load roles.');
          this.loadingRoles.set(false);
        },
      });
  }

  private loadRoleClaims(roleId: string): void {
    this.loadingClaims.set(true);
    this.errorMessage.set(null);

    this.roleService
      .getRoleClaims(roleId)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (response) => {
          this.claims.set(response.data ?? []);
          this.loadingClaims.set(false);
        },
        error: () => {
          this.errorMessage.set('Failed to load role claims.');
          this.claims.set([]);
          this.loadingClaims.set(false);
        },
      });
  }
}
