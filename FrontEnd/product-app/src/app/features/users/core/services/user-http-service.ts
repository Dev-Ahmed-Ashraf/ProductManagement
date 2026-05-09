import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../core/Models/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { GetUsersQuery } from '../models/get-users-query.model';
import { UserResponse } from '../models/user-response.model';
import { PagedResponse } from '../../../../core/Models/paged-response.model';
import { ApiResponse } from '../../../../core/Models/GenericResponse.model';
import { CreateUserCommand } from '../models/create-user-command.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class UserHttpService {
  private http = inject(HttpClient);

  private apiUrl = `${environment.apiUrl}/Users`;

  getUsers(query: GetUsersQuery) {
    let params = new HttpParams();

    if (query.pageNumber !== null && query.pageNumber !== undefined) {
      params = params.set('pageNumber', String(query.pageNumber));
    }

    if (query.pageSize !== null && query.pageSize !== undefined) {
      params = params.set('pageSize', String(query.pageSize));
    }

    if (query.name) {
      params = params.set('name', query.name);
    }

    if (query.userName) {
      params = params.set('userName', query.userName);
    }

    if (query.email !== null && query.email !== undefined) {
      params = params.set('email', String(query.email));
    }

    if (query.role !== null && query.role !== undefined) {
      params = params.set('role', String(query.role));
    }

    if (query.isActive !== null && query.isActive !== undefined) {
      params = params.set('isActive', String(query.isActive));
    }

    return this.http.get<ApiResponse<PagedResponse<UserResponse>>>(this.apiUrl, { params });
  }

  createUser(data: CreateUserCommand): import('rxjs').Observable<ApiResponse<UserResponse>> {
    return this.http.post<ApiResponse<UserResponse>>(`${this.apiUrl}`, data);
  }

  getUserById(id: string): Observable<ApiResponse<UserResponse>> {
    return this.http.get<ApiResponse<UserResponse>>(`${this.apiUrl}/${id}`);
  }
}
