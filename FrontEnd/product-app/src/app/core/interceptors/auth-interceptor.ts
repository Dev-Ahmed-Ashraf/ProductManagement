import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth/auth';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const accessToken = authService.getAccessToken();

  const isAuthRequest =
    req.url.includes('/api/Auth/login') || req.url.includes('/api/Auth/refresh-token');

  const authenticatedRequest =
    !isAuthRequest && accessToken
      ? req.clone({ setHeaders: { Authorization: `Bearer ${accessToken}` } })
      : req;

  return next(authenticatedRequest).pipe(
    catchError((error: HttpErrorResponse) => {
      console.debug('[auth-interceptor] caught error', {
        status: error?.status,
        url: req.url,
        isAuthRequest,
      });
      if (error.status !== 401 || isAuthRequest) {
        return throwError(() => error);
      }

      console.debug('[auth-interceptor] attempting refresh for', req.url);
      return authService.refreshAccessToken().pipe(
        switchMap((refreshResponse) => {
          console.debug('[auth-interceptor] refresh response', refreshResponse);
          const session = refreshResponse.data;

          if (!session?.accessToken) {
            console.debug('[auth-interceptor] refresh missing accessToken');
            return handleRefreshFailure(
              authService,
              router,
              'Missing access token in refresh response',
            );
          }

          authService.setSession(session, true);

          const retriedRequest = authenticatedRequest.clone({
            setHeaders: {
              Authorization: `Bearer ${session.accessToken}`,
            },
          });

          console.debug('[auth-interceptor] retrying request with new token', { url: req.url });
          return next(retriedRequest);
        }),
        catchError((refreshError) => {
          console.debug('[auth-interceptor] refresh failed', refreshError);
          return handleRefreshFailure(authService, router, refreshError);
        }),
      );
    }),
  );
};

function handleRefreshFailure(authService: AuthService, router: Router, error: unknown) {
  authService.logout();
  void router.navigate(['/login']);
  return throwError(() => error);
}
