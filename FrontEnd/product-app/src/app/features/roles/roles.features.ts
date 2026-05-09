export interface RolesFeatureItem {
  title: string;
  route: string;
  permission: string;
  icon?: 'list' | 'plus' | 'status';
}

export const ROLES_FEATURES: RolesFeatureItem[] = [
  {
    title: 'Roles List',
    route: '/dashboard/roles',
    permission: 'roles:view',
    icon: 'list',
  },
  {
    title: 'Role Claims',
    route: '/dashboard/role-claims',
    permission: 'roles:view',
    icon: 'status',
  },
];
