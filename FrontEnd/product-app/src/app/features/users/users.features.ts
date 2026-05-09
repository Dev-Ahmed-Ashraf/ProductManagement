export interface UsersFeatureItem {
  title: string;
  route: string;
  permission: string;
  icon?: 'list' | 'plus' | 'status';
}

export const USERS_FEATURES: UsersFeatureItem[] = [
  {
    title: 'Users List',
    route: '/dashboard/users',
    permission: 'users:view',
    icon: 'list',
  },
  {
    title: 'Add User',
    route: '/dashboard/users/add',
    permission: 'users:create',
    icon: 'plus',
  },
];
