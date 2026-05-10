import { inject } from '@angular/core';
import { CanActivateFn, Router, UrlTree } from '@angular/router';
import { AuthService } from '../services/auth/auth';

// Guard to protect routes that require specific permissions
export const permissionGuard: CanActivateFn = (route, state) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  const required = route.data?.['permission'];
  const mode = route.data?.['mode'] || 'all';

  let perms: string[] = [];

  // Normalize the required permissions into an array
  if (typeof required === 'string') perms = [required];
  else if (Array.isArray(required)) perms = required;

  if (perms.length === 0) return true;

  // Helper function to check if the user has a specific permission
  const has = (p: string) => auth.hasPermission(p);

  // 'any' means the user needs at least one of the permissions, 'all' means they need all of them
  const ok = mode === 'any' ? perms.some(has) : perms.every(has);

  // If the user has the required permissions, allow access to the route
  return ok ? true : router.parseUrl('/forbidden');
};
