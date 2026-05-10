import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth/auth';

// Guard to protect routes that require authentication
export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // If user is logged in, allow access
  if (authService.isLoggedIn()) {
    return true;
  }

  // Not logged in, redirect to login page
  return router.createUrlTree(['/login']);
};
