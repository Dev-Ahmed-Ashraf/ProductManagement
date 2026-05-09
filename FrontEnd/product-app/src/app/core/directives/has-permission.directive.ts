import { Directive, Input, OnDestroy, TemplateRef, ViewContainerRef, inject } from '@angular/core';
import { AuthService } from '../services/auth/auth';
import { Subject, takeUntil } from 'rxjs';

@Directive({
  selector: '[hasPermission]', // Usage: <div *hasPermission="'permission:string'">...</div> or <div *hasPermission="['perm1', 'perm2']">...</div>
})
export class HasPermissionDirective implements OnDestroy {
  private auth = inject(AuthService);
  private destroy$ = new Subject<void>(); // Used to clean up the subscription when the directive is destroyed
  private hasView = false;
  private requiredPermissions: string[] = [];

  @Input('hasPermission')
  set required(value: string | string[] | null | undefined) {
    const permissions = Array.isArray(value) ? value : value ? [value] : [];
    this.requiredPermissions = permissions;
    this.updateView();
  }

  constructor(
    private template: TemplateRef<any>,
    private vcr: ViewContainerRef,
  ) {
    this.auth.permissions$.pipe(takeUntil(this.destroy$)).subscribe(() => this.updateView()); // Update the view whenever permissions change
  }

  // Check if the user has the required permissions
  private check(): boolean {
    if (!this.requiredPermissions.length) return false;

    return this.requiredPermissions.every((permission) => this.auth.hasPermission(permission));
  }

  private updateView() {
    const ok = this.check();

    if (ok && !this.hasView) {
      this.vcr.createEmbeddedView(this.template);
      this.hasView = true;
    } else if (!ok && this.hasView) {
      this.vcr.clear();
      this.hasView = false;
    }
  }

  // Clean up the subscription when the directive is destroyed
  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
