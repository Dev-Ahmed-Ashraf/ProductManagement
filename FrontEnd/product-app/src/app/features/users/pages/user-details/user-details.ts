import { CommonModule } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  OnInit,
  computed,
  inject,
  signal,
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { UserResponse } from '../../core/models/user-response.model';
import { UserService } from '../../core/services/user-service';

@Component({
  selector: 'app-user-details',
  standalone: true,
  imports: [CommonModule, RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './user-details.html',
  styleUrl: './user-details.css',
})
export class UserDetails implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly userService = inject(UserService);
  private readonly destroyRef = inject(DestroyRef);

  readonly loading = signal(true);
  readonly errorMessage = signal<string | null>(null);
  readonly user = signal<UserResponse | null>(null);
  readonly userInitials = computed(() => {
    const fullName = this.user()?.fullName?.trim();

    if (!fullName) {
      return 'U';
    }

    const [first = '', second = ''] = fullName.split(/\s+/);
    return `${first.charAt(0)}${second.charAt(0) || first.charAt(1) || ''}`.toUpperCase();
  });

  ngOnInit(): void {
    this.route.paramMap.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((params) => {
      const userId = params.get('id');

      if (!userId) {
        this.user.set(null);
        this.errorMessage.set('Invalid user identifier.');
        this.loading.set(false);
        return;
      }

      this.loadUser(userId);
    });
  }

  private loadUser(userId: string): void {
    this.loading.set(true);
    this.errorMessage.set(null);

    this.userService
      .getUserById(userId)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (response) => {
          this.user.set(response.data ?? null);
          this.loading.set(false);
        },
        error: () => {
          this.user.set(null);
          this.errorMessage.set('Failed to load user details.');
          this.loading.set(false);
        },
      });
  }
}
