import {
  HttpErrorResponse,
  HttpInterceptorFn,
  HttpRequest,
  HttpResponse,
} from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { firstValueFrom, of, throwError } from 'rxjs';

import { httpInterceptor } from './http-interceptor';
import { ErrorHandlerService } from '../services/error-handler/error-handler.service';
import { AuthService } from '../services/auth/auth';

describe('httpInterceptor', () => {
  const authServiceSpy = {
    getAccessToken: vi.fn(),
  } as unknown as AuthService;

  const errorHandlerSpy = {
    showError: vi.fn(),
  } as unknown as ErrorHandlerService;

  const interceptor: HttpInterceptorFn = (req, next) =>
    TestBed.runInInjectionContext(() => httpInterceptor(req, next));

  beforeEach(() => {
    vi.mocked(authServiceSpy.getAccessToken).mockReturnValue('test-token');
    vi.mocked(errorHandlerSpy.showError).mockReset();

    TestBed.configureTestingModule({
      providers: [
        { provide: AuthService, useValue: authServiceSpy },
        { provide: ErrorHandlerService, useValue: errorHandlerSpy },
      ],
    });
  });

  it('should be created', () => {
    expect(interceptor).toBeTruthy();
  });

  it('should add the auth header for protected requests', async () => {
    const request = new HttpRequest('POST', '/api/Product', { name: 'Test product' });

    await firstValueFrom(
      interceptor(request, (nextRequest) => {
        expect(nextRequest.headers.get('Authorization')).toBe('Bearer test-token');
        expect(nextRequest.headers.get('Content-Type')).toBe('application/json');

        return of(new HttpResponse({ status: 200 }));
      }),
    );
  });

  it('should skip the auth header for login requests', async () => {
    const request = new HttpRequest('POST', '/api/Auth/login', { email: 'test@example.com' });

    await firstValueFrom(
      interceptor(request, (nextRequest) => {
        expect(nextRequest.headers.has('Authorization')).toBe(false);
        expect(nextRequest.headers.get('Content-Type')).toBe('application/json');

        return of(new HttpResponse({ status: 200 }));
      }),
    );
  });

  it('should not show an error for 401 responses (handled by auth-interceptor)', async () => {
    const request = new HttpRequest('GET', '/api/Product');

    await expect(
      firstValueFrom(
        interceptor(request, () =>
          throwError(
            () =>
              new HttpErrorResponse({
                status: 401,
                statusText: 'Unauthorized',
                url: '/api/Product',
              }),
          ),
        ),
      ),
    ).rejects.toMatchObject({ status: 401 });

    expect(errorHandlerSpy.showError).not.toHaveBeenCalled();
  });
});
