import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  OnInit,
  inject,
  signal,
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { RoleResponse } from '../../core/models/role-response.model';
import { RoleService } from '../../core/service/role-service';

@Component({
  selector: 'app-roles-list',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink],
  templateUrl: './roles-list.html',
  styleUrl: './roles-list.css',
})
export class RolesList implements OnInit {
  private readonly roleService = inject(RoleService);
  private readonly destroyRef = inject(DestroyRef);

  readonly roles = signal<RoleResponse[]>([]);
  readonly loading = signal(false);
  readonly errorMessage = signal<string | null>(null);

  ngOnInit(): void {
    this.loadRoles();
  }

  trackByRole(_: number, role: RoleResponse): number {
    return role.id;
  }

  private loadRoles(): void {
    this.loading.set(true);
    this.errorMessage.set(null);

    this.roleService
      .getRoles()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (response) => {
          this.roles.set(response.data ?? []);
          this.loading.set(false);
        },
        error: () => {
          this.errorMessage.set('Failed to load roles. Please try again.');
          this.roles.set([]);
          this.loading.set(false);
        },
      });
  }
}
