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
import {
  FormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { finalize } from 'rxjs';
import { RoleDropdown } from '../../../../shared/components/role-dropdown/role-dropdown';
import { RoleResponse } from '../../../roles/core/models/role-response.model';
import { RoleClaimsResponse } from '../../../roles/core/models/role-claims-response.model';
import { RoleService } from '../../../roles/core/service/role-service';
import { UserService } from '../../core/services/user-service';
import { AlertService } from '../../../../core/services/alert/alert';

function passwordsMatchValidator(): ValidatorFn {
  return (control): ValidationErrors | null => {
    const password = control.get('password')?.value;
    const confirmPassword = control.get('confirmPassword')?.value;

    if (!password || !confirmPassword) {
      return null;
    }

    return password === confirmPassword ? null : { passwordMismatch: true };
  };
}

interface PasswordStrengthState {
  label: string;
  score: number;
  color: string;
}

@Component({
  selector: 'app-add-user',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, ReactiveFormsModule, RoleDropdown],
  templateUrl: './add-user.html',
  styleUrl: './add-user.css',
})
export class AddUser implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly userService = inject(UserService);
  private readonly roleService = inject(RoleService);
  private readonly alert = inject(AlertService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  readonly loadingRoles = signal(false);
  readonly loadingClaims = signal(false);
  readonly submitting = signal(false);
  readonly roles = signal<RoleResponse[]>([]);
  readonly roleClaims = signal<RoleClaimsResponse[]>([]);
  readonly passwordValue = signal('');
  readonly selectedRoleId = signal<string | null>(null);
  readonly showPassword = signal(false);

  readonly passwordStrength = computed<PasswordStrengthState>(() => {
    const value = this.passwordValue();
    let score = 0;

    if (value.length >= 8) score += 1;
    if (/[a-z]/.test(value) && /[A-Z]/.test(value)) score += 1;
    if (/\d/.test(value)) score += 1;
    if (/[^A-Za-z\d]/.test(value)) score += 1;

    if (score <= 1) {
      return { label: 'Weak', score, color: 'bg-rose-500' };
    }

    if (score === 2 || score === 3) {
      return { label: 'Fair', score, color: 'bg-amber-500' };
    }

    return { label: 'Strong', score, color: 'bg-emerald-500' };
  });

  readonly hasRoleClaims = computed(() => this.roleClaims().length > 0);

  readonly form = this.fb.group(
    {
      fullName: this.fb.nonNullable.control('', [Validators.required, Validators.minLength(3)]),
      email: this.fb.nonNullable.control('', [Validators.required, Validators.email]),
      password: this.fb.nonNullable.control('', [Validators.required, Validators.minLength(8)]),
      confirmPassword: this.fb.nonNullable.control('', [Validators.required]),
      roleId: this.fb.control<string | null>(null, [Validators.required]),
    },
    { validators: [passwordsMatchValidator()] },
  );

  ngOnInit(): void {
    this.bindPasswordStrength();
    this.bindRoleSelection();
    this.loadRoles();
  }

  get fullNameControl() {
    return this.form.controls.fullName;
  }

  get emailControl() {
    return this.form.controls.email;
  }

  get passwordControl() {
    return this.form.controls.password;
  }

  get confirmPasswordControl() {
    return this.form.controls.confirmPassword;
  }

  get roleIdControl() {
    return this.form.controls.roleId;
  }

  onRoleChange(roleId: string | null): void {
    this.roleIdControl.setValue(roleId);
    this.roleIdControl.markAsTouched();
  }

  togglePasswordVisibility(): void {
    this.showPassword.update((value) => !value);
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const roleId = this.roleIdControl.value;
    const role = this.roles().find((item) => item.id.toString() === roleId);

    if (!role) {
      this.roleIdControl.setErrors({ required: true });
      return;
    }

    this.submitting.set(true);

    this.userService
      .createUser({
        fullName: this.fullNameControl.value.trim(),
        email: this.emailControl.value.trim(),
        password: this.passwordControl.value,
        confirmPassword: this.confirmPasswordControl.value,
        role: role.name,
      })
      .pipe(
        finalize(() => this.submitting.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (response) => {
          this.alert.success('User created successfully.');
          const createdUserId = response.data?.id;

          this.form.reset();
          this.roleClaims.set([]);
          this.selectedRoleId.set(null);

          if (createdUserId) {
            void this.router.navigate(['/dashboard/users', createdUserId]);
          }
        },
      });
  }

  trackByClaim(_: number, claim: RoleClaimsResponse): string {
    return `${claim.type}:${claim.value}`;
  }

  private bindPasswordStrength(): void {
    this.passwordControl.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((value) => {
        this.passwordValue.set(value);
      });

    this.passwordValue.set(this.passwordControl.value);
  }

  private bindRoleSelection(): void {
    this.roleIdControl.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((roleId) => {
        this.selectedRoleId.set(roleId);

        if (roleId === null) {
          this.roleClaims.set([]);
          return;
        }

        this.loadRoleClaims(roleId);
      });
  }

  private loadRoles(): void {
    this.loadingRoles.set(true);

    this.roleService
      .getRoles()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (response) => {
          this.roles.set(response.data ?? []);
          this.loadingRoles.set(false);
        },
        error: () => {
          this.roles.set([]);
          this.loadingRoles.set(false);
        },
      });
  }

  private loadRoleClaims(roleId: string): void {
    this.loadingClaims.set(true);

    this.roleService
      .getRoleClaims(roleId)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (response) => {
          this.roleClaims.set(response.data ?? []);
          this.loadingClaims.set(false);
        },
        error: () => {
          this.roleClaims.set([]);
          this.loadingClaims.set(false);
        },
      });
  }
}
