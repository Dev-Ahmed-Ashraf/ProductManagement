import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { AuthService } from '../../../../../core/services/auth/auth';
import { DashboardSidebar } from '../../../sidebar/sidebar';
import {
  filterProductFeatures,
  filterProductActions,
  PRODUCT_FEATURES,
  PRODUCT_DETAIL_ACTIONS,
} from '../../../../products/products.features';
import { ROLES_FEATURES } from '../../../../roles/roles.features';

@Component({
  selector: 'app-dashboard',
  imports: [DashboardSidebar, RouterOutlet],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css'],
})
export class Dashboard {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  readonly currentUser = this.auth.user;

  private readonly permissions = toSignal(this.auth.permissions$, {
    initialValue: new Set<string>(),
  });

  readonly userDisplayName = computed(() => this.currentUser()?.fullName || 'Workspace User');
  readonly userRole = computed(() => this.currentUser()?.roles?.join(', ') || 'Workspace member');

  readonly visibleFeatures = computed(() => {
    const permissions = this.permissions();
    return [
      ...filterProductFeatures(permissions, PRODUCT_FEATURES),
      ...ROLES_FEATURES.filter((feature) => permissions.has(feature.permission.toLowerCase())),
    ];
  });

  readonly visibleActions = computed(() =>
    filterProductActions(this.permissions(), PRODUCT_DETAIL_ACTIONS),
  );

  readonly hasVisibleFeatures = computed(() => this.visibleFeatures().length > 0);

  logout(): void {
    this.auth.logout();
    void this.router.navigate(['/login']);
  }
}
