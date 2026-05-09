import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  computed,
  inject,
  signal,
} from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../../core/services/auth/auth';
import { finalize, take } from 'rxjs';
import { AlertService } from '../../../../core/services/alert/alert';

@Component({
  selector: 'app-login',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login implements OnInit {
  private readonly strongPasswordPattern = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d]).+$/;

  private fb = inject(NonNullableFormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);
  private alert = inject(AlertService);

  readonly isLoading = signal(false);
  readonly showPassword = signal(false);
  readonly buttonLabel = computed(() => (this.isLoading() ? 'Signing in...' : 'Sign in'));

  loginForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: [
      '',
      [
        Validators.required,
        Validators.minLength(8),
        Validators.pattern(this.strongPasswordPattern),
      ],
    ],
    rememberMe: [true],
  });

  ngOnInit() {
    if (this.authService.isLoggedIn()) {
      void this.router.navigate(['/dashboard']);
    }
  }

  togglePasswordVisibility() {
    this.showPassword.update((value) => !value);
  }

  onSubmit() {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    const { email, password, rememberMe } = this.loginForm.getRawValue();

    this.isLoading.set(true);

    this.authService
      .login({ email, password })
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: (response) => {
          const isSessionStored = this.authService.setSession(response.data, rememberMe);

          if (!isSessionStored) {
            return;
          }

          this.alert.success(
            `Welcome back, ${response.data?.fullName || response.data?.email}`,
            'Login successful',
          );
          void this.router.navigate(['/dashboard']);
        },
      });
  }
}
