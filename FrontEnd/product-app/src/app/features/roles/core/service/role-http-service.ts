import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../core/Models/environment';
import { HttpClient } from '@angular/common/http';
import { ApiResponse } from '../../../../core/Models/GenericResponse.model';
import { RoleResponse } from '../models/role-response.model';
import { RoleClaimsResponse } from '../models/role-claims-response.model';

@Injectable({
  providedIn: 'root',
})
export class RoleHttpService {
  private http = inject(HttpClient);

  private apiUrl = `${environment.apiUrl}/Roles`;

  getRoles() {
    return this.http.get<ApiResponse<RoleResponse[]>>(`${this.apiUrl}`);
  }

  getRoleClaims(id: string) {
    return this.http.get<ApiResponse<RoleClaimsResponse[]>>(`${this.apiUrl}/${id}/claims`);
  }
}
