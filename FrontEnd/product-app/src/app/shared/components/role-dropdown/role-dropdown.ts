import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { RoleResponse } from '../../../features/roles/core/models/role-response.model';

@Component({
  selector: 'app-role-dropdown',
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './role-dropdown.html',
  styleUrl: './role-dropdown.css',
})
export class RoleDropdown {
  readonly roles = input<RoleResponse[]>([]);
  readonly selectedRoleId = input<string | null>(null);
  readonly disabled = input(false);
  readonly label = input('Select Role');
  readonly emptyOptionLabel = input('Select a role');

  readonly roleChange = output<string | null>();

  onChange(nextValue: string): void {
    if (!nextValue) {
      this.roleChange.emit(null);
      return;
    }

    const parsed = nextValue;
    this.roleChange.emit(parsed);
  }
}
