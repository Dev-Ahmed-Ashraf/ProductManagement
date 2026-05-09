import { isPlatformBrowser } from '@angular/common';
import { inject, Injectable, PLATFORM_ID, signal } from '@angular/core';
import {
  BehaviorSubject,
  catchError,
  filter,
  finalize,
  Observable,
  of,
  take,
  tap,
  throwError,
} from 'rxjs';
import { jwtDecode, JwtPayload } from 'jwt-decode';
import { ApiResponse } from '../../Models/GenericResponse.model';
import { LoginRequest } from '../../../features/auth/Core/Models/login-request.model';
import { LoginResponse } from '../../../features/auth/Core/Models/login-response.model';
import { AuthHttpService } from '../../../features/auth/Core/Service/AuthHttpService';

interface AuthTokenClaims extends JwtPayload {
  email?: string;
  fullName?: string;
  role?: string | string[];
  roles?: string | string[];
  permission_json?: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private platformId = inject(PLATFORM_ID);
  private authHttpService = inject(AuthHttpService);

  private refreshInFlight = false;
  private refreshQueue$ = new BehaviorSubject<ApiResponse<LoginResponse> | null>(null);

  private permissionsSubject = new BehaviorSubject<Set<string>>(new Set());
  readonly permissions$ = this.permissionsSubject.asObservable();

  STORAGE_KEYS = {
    session: 'session',
    accessToken: 'accessToken',
    refreshToken: 'refreshToken',
  };

  initAuth(): void {
    const token = this.getAccessToken();

    if (!token) return;

    this.permissionsSubject.next(this.extractPermissions());

    this.currentUser.set(this.getCurrentUser());
  }

  private decodeToken(): AuthTokenClaims | null {
    const token = this.getAccessToken();

    if (!token) return null;

    try {
      return jwtDecode<AuthTokenClaims>(token);
    } catch {
      return null;
    }
  }

  private extractPermissionsFromToken(): Set<string> {
    const claims = this.decodeToken();
    if (!claims) return new Set();

    try {
      const raw = claims.permission_json;
      if (!raw) return new Set();

      const parsed: string[] = JSON.parse(raw); // convert to array of strings

      return new Set(parsed.map((p) => p.toLowerCase())); // store permissions in lowercase for case-insensitive comparison
    } catch {
      return new Set();
    }
  }

  private extractPermissions(): Set<string> {
    const tokenPerms = this.extractPermissionsFromToken();

    if (tokenPerms.size > 0) {
      return tokenPerms;
    }

    return this.normalizePermissions(this.getStoredSession()?.claims);
  }

  getCurrentUser() {
    const claims = this.decodeToken();

    if (!claims) return null;

    const session = this.getStoredSession();

    return {
      userId: claims.sub,
      fullName: session?.fullName,
      roles: session?.roles,
    };
  }

  private readonly currentUser = signal(this.getCurrentUser());
  readonly user = this.currentUser.asReadonly();

  getPermissions(): string[] {
    return Array.from(this.permissionsSubject.getValue());
  }

  hasPermission(permission: string): boolean {
    if (!permission) return false;
    return this.permissionsSubject.getValue().has(permission.toLowerCase());
  }

  login(credentials: LoginRequest): Observable<ApiResponse<LoginResponse>> {
    return this.authHttpService.login(credentials);
  }

  getRefreshToken(): string | null {
    if (!isPlatformBrowser(this.platformId)) return null;
    return (
      sessionStorage.getItem(this.STORAGE_KEYS.refreshToken) ||
      localStorage.getItem(this.STORAGE_KEYS.refreshToken)
    );
  }

  refreshAccessToken(): Observable<ApiResponse<LoginResponse>> {
    if (!isPlatformBrowser(this.platformId)) {
      return throwError(() => new Error('Refresh token is not available outside the browser.'));
    }

    const refreshToken = this.getRefreshToken();

    if (!refreshToken) {
      return throwError(() => new Error('No refresh token available in Current Context.'));
    }

    if (this.refreshInFlight) {
      return this.refreshQueue$.pipe(
        filter((response): response is ApiResponse<LoginResponse> => !!response),
        take(1),
      );
    }

    this.refreshInFlight = true;
    this.refreshQueue$.next(null);

    return this.authHttpService.refreshToken({ refreshToken }).pipe(
      tap((response) => {
        const session = response.data;

        if (session) {
          const storage = this.getPreferredStorage();

          if (storage) {
            this.saveSession(storage, session);
          }

          this.permissionsSubject.next(this.extractPermissions());
          this.currentUser.set(this.getCurrentUser());
          this.refreshQueue$.next(response);
        }
      }),
      catchError((error) => {
        this.clearSession();
        this.currentUser.set(null);
        this.refreshQueue$.next(null);
        return throwError(() => error);
      }),
      finalize(() => {
        this.refreshInFlight = false;
      }),
    );
  }

  logout(): void {
    this.clearSession();
    this.currentUser.set(null);
  }

  isLoggedIn() {
    const token = this.getAccessToken();

    if (!token) return false;

    return !this.isTokenExpired(token);
  }

  isTokenExpired(token: string) {
    try {
      const payload = jwtDecode<JwtPayload>(token);
      const exp = payload.exp;

      if (!exp) {
        return true;
      }

      const now = Math.floor(Date.now() / 1000);
      return exp < now;
    } catch {
      return true;
    }
  }

  getAccessToken(): string | null {
    if (!isPlatformBrowser(this.platformId)) return null;
    return (
      sessionStorage.getItem(this.STORAGE_KEYS.accessToken) ||
      localStorage.getItem(this.STORAGE_KEYS.accessToken)
    );
  }

  setSession(response: LoginResponse | undefined, rememberMe: boolean): boolean {
    if (!isPlatformBrowser(this.platformId)) {
      return false;
    }

    this.clearSession();

    if (!response || !response.accessToken) {
      return false;
    }

    const storage = rememberMe ? localStorage : sessionStorage;
    this.saveSession(storage, response);
    this.currentUser.set(this.getCurrentUser());

    this.permissionsSubject.next(this.extractPermissions());

    return true;
  }

  clearSession(): void {
    if (!isPlatformBrowser(this.platformId)) {
      return;
    }

    localStorage.removeItem(this.STORAGE_KEYS.session);
    sessionStorage.removeItem(this.STORAGE_KEYS.session);

    localStorage.removeItem(this.STORAGE_KEYS.accessToken);
    localStorage.removeItem(this.STORAGE_KEYS.refreshToken);

    sessionStorage.removeItem(this.STORAGE_KEYS.accessToken);
    sessionStorage.removeItem(this.STORAGE_KEYS.refreshToken);

    this.permissionsSubject.next(new Set());
  }

  private saveSession(storage: Storage, response: LoginResponse): void {
    storage.setItem(this.STORAGE_KEYS.session, JSON.stringify(response));
    storage.setItem(this.STORAGE_KEYS.accessToken, response.accessToken);
    if (response.refreshToken) {
      storage.setItem(this.STORAGE_KEYS.refreshToken, response.refreshToken);
    }
  }

  private getPreferredStorage(): Storage | null {
    if (!isPlatformBrowser(this.platformId)) return null;

    if (sessionStorage.getItem(this.STORAGE_KEYS.session)) {
      return sessionStorage;
    }

    if (localStorage.getItem(this.STORAGE_KEYS.session)) {
      return localStorage;
    }

    return sessionStorage;
  }

  private getStoredSession(): LoginResponse | null {
    if (!isPlatformBrowser(this.platformId)) return null;

    const raw =
      sessionStorage.getItem(this.STORAGE_KEYS.session) ||
      localStorage.getItem(this.STORAGE_KEYS.session);

    if (!raw) return null;

    try {
      return JSON.parse(raw) as LoginResponse;
    } catch {
      return null;
    }
  }

  private normalizeRoles(value: string | string[] | undefined): string[] {
    if (!value) return [];

    if (Array.isArray(value)) {
      return value.filter(Boolean);
    }

    return value
      .split(',')
      .map((role) => role.trim())
      .filter(Boolean);
  }

  private normalizePermissions(value: string[] | undefined): Set<string> {
    return new Set((value ?? []).filter(Boolean).map((permission) => permission.toLowerCase()));
  }

  tryRestoreSession(): Observable<any> {
    const refreshToken = this.getRefreshToken();

    if (!refreshToken) {
      return of(null);
    }

    return this.refreshAccessToken().pipe(
      tap((res) => {
        this.setSession(res.data, true);
      }),
      catchError(() => {
        this.logout();
        return of(null);
      }),
    );
  }
}
