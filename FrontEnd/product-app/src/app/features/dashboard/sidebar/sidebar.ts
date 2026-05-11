import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { AuthService } from '../../../core/services/auth/auth';
import { PRODUCT_FEATURES } from '../../products/products.features';
import { USERS_FEATURES } from '../../users/users.features';
import { ROLES_FEATURES } from '../../roles/roles.features';

@Component({
  selector: 'app-dashboard-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.css',
})
export class DashboardSidebar {
  private readonly auth = inject(AuthService);

  private readonly permissions = toSignal(this.auth.permissions$, {
    initialValue: new Set<string>(),
  });

  readonly visibleFeatures = computed(() => {
    const allFeatures = [...PRODUCT_FEATURES, ...USERS_FEATURES, ...ROLES_FEATURES];
    const permissions = this.permissions();

    return allFeatures.filter((feature) => permissions.has(feature.permission.toLowerCase()));
  });
  readonly groupedFeatures = computed(() => {
    const permissions = this.permissions();

    const all = [...PRODUCT_FEATURES, ...USERS_FEATURES, ...ROLES_FEATURES].filter((f) =>
      permissions.has(f.permission.toLowerCase()),
    );

    const groups: { label: string; items: any[] }[] = [];

    const pushGroup = (label: string, items: any[]) => {
      if (items.length) groups.push({ label, items });
    };

    // Product-related groups
    const productItems = all.filter((f) => f.route?.startsWith('/dashboard/products'));
    const historyItems = all.filter((f) => f.route?.startsWith('/dashboard/product-histories'));
    pushGroup('Products', productItems);
    pushGroup('Histories', historyItems);

    // Users and Roles
    const userItems = all.filter((f) => f.route?.startsWith('/dashboard/users'));
    const roleItems = all.filter((f) => f.route?.startsWith('/dashboard/roles'));
    pushGroup('Users', userItems);
    pushGroup('Roles', roleItems);

    const roleClaimItems = all.filter((f) => f.route?.startsWith('/dashboard/role-claims'));
    pushGroup('Role Claims', roleClaimItems);
    // Any other items (fallback)
    const other = all.filter(
      (f) =>
        !productItems.includes(f) &&
        !historyItems.includes(f) &&
        !userItems.includes(f) &&
        !roleItems.includes(f) &&
        !roleClaimItems.includes(f),
    );
    pushGroup('Other', other);

    return groups;
  });

  readonly hasVisibleFeatures = computed(() =>
    this.groupedFeatures().some((g) => g.items.length > 0),
  );

  trackByRoute(_: number, feature: { route: string }): string {
    return feature.route;
  }

  groupTrack(_: number, group: { label: string }): string {
    return group.label;
  }

  iconFor(icon: string): string {
    switch (icon) {
      case 'plus':
      case 'status':
        return 'M12 5v14m-7-7h14';
      case 'trash':
        return 'M3 6h18M8 6V4h8v2M6 6l1 14h10l1-14M10 11v5M14 11v5';
      default:
        return 'M4 7.5A2.5 2.5 0 0 1 6.5 5H9l1.5 2H17.5A2.5 2.5 0 0 1 20 9.5v8A2.5 2.5 0 0 1 17.5 20h-11A2.5 2.5 0 0 1 4 17.5v-10Z M8 12h8';
    }
  }
}
