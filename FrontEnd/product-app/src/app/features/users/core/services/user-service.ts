import { inject, Injectable } from '@angular/core';
import { UserHttpService } from './user-http-service';
import { GetUsersQuery } from '../models/get-users-query.model';
import { UserResponse } from '../models/user-response.model';
import { PagedResponse } from '../../../../core/Models/paged-response.model';
import { ApiResponse } from '../../../../core/Models/GenericResponse.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private userHttpService = inject(UserHttpService);

  getUsers(query: GetUsersQuery): Observable<ApiResponse<PagedResponse<UserResponse>>> {
    return this.userHttpService.getUsers(query);
  }

  createUser(
    data: import('../models/create-user-command.model').CreateUserCommand,
  ): Observable<ApiResponse<UserResponse>> {
    return this.userHttpService.createUser(data);
  }

  getUserById(id: string): Observable<ApiResponse<UserResponse>> {
    return this.userHttpService.getUserById(id);
  }
}
