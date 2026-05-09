import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../core/Models/environment';
import { ApiResponse } from '../../../../core/Models/GenericResponse.model';
import { LoginRequest } from '../Models/login-request.model';
import { LoginResponse } from '../Models/login-response.model';
import { RefreshTokenRequest } from '../Models/refresh-token-request.model';

@Injectable({
  providedIn: 'root',
})
export class AuthHttpService {
  private http = inject(HttpClient);

  private apiUrl = `${environment.apiUrl}/Auth`;

  login(credentials: LoginRequest) {
    return this.http.post<ApiResponse<LoginResponse>>(`${this.apiUrl}/login`, credentials);
  }

  refreshToken(refreshTokenReq: RefreshTokenRequest) {
    return this.http.post<ApiResponse<LoginResponse>>(
      `${this.apiUrl}/refresh-token`,
      refreshTokenReq,
    );
  }
}
