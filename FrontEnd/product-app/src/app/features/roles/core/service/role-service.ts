import { inject, Injectable } from '@angular/core';
import { RoleHttpService } from './role-http-service';

@Injectable({
  providedIn: 'root',
})
export class RoleService {
  private roleHttpService = inject(RoleHttpService);

  getRoles() {
    return this.roleHttpService.getRoles();
  }

  getRoleClaims(id: string) {
    return this.roleHttpService.getRoleClaims(id);
  }
}
