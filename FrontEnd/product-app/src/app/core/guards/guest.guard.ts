import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth/auth';

// Guard to prevent authenticated users from accessing routes meant for guests (e.g., login, register)
export const guestGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // If the user is not logged in, allow access to the route
  if (!authService.isLoggedIn()) {
    return true;
  }

  // If the user is logged in, redirect to the dashboard
  return router.createUrlTree(['/dashboard']);
};
