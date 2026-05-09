import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth/auth';

const LANDING_ROUTES = [
  { route: '/dashboard/statistics', permission: 'statistics:view' },
  { route: '/dashboard/products', permission: 'products:view' },
  { route: '/dashboard/product-histories', permission: 'product-status-histories:view' },
  { route: '/dashboard/products/add', permission: 'products:create' },
  { route: '/dashboard/users', permission: 'users:view' },
  { route: '/dashboard/users/add', permission: 'users:create' },
  { route: '/dashboard/roles', permission: 'roles:view' },
];

export const dashboardLandingGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  const landingRoute = LANDING_ROUTES.find((item) => auth.hasPermission(item.permission));

  return router.parseUrl(landingRoute?.route ?? '/forbidden');
};
