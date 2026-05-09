import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { guestGuard } from './core/guards/guest.guard';
import { Unauthorized } from './shared/components/unauthorized/unauthorized';
import { permissionGuard } from './core/guards/permission.guard';
import { dashboardLandingGuard } from './core/guards/dashboard-landing.guard';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'login',
  },
  {
    path: 'dashboard',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/dashboard/dashboard/pages/dashboard/dashboard').then((m) => m.Dashboard),
    children: [
      {
        path: '',
        pathMatch: 'full',
        canActivate: [dashboardLandingGuard],
        children: [],
      },
      {
        path: 'products',
        canActivate: [permissionGuard],
        data: { permission: 'products:view' },
        loadComponent: () =>
          import('./features/products/Pages/product-list/product-list').then((m) => m.ProductList),
      },
      {
        path: 'statistics',
        canActivate: [permissionGuard],
        data: { permission: 'statistics:view' },
        loadComponent: () =>
          import('./features/dashboard/dashboard/pages/statistics/statistics').then(
            (m) => m.StatisticsPage,
          ),
      },
      {
        path: 'users',
        canActivate: [permissionGuard],
        data: { permission: 'users:view' },
        loadComponent: () =>
          import('./features/users/pages/users-list/users-list').then((m) => m.UsersList),
      },
      {
        path: 'users/add',
        canActivate: [permissionGuard],
        data: { permission: 'users:create' },
        loadComponent: () =>
          import('./features/users/pages/add-user/add-user').then((m) => m.AddUser),
      },
      {
        path: 'users/:id',
        canActivate: [permissionGuard],
        data: { permission: 'users:view' },
        loadComponent: () =>
          import('./features/users/pages/user-details/user-details').then((m) => m.UserDetails),
      },
      {
        path: 'roles',
        canActivate: [permissionGuard],
        data: { permission: 'roles:view' },
        loadComponent: () =>
          import('./features/roles/pages/roles-list/roles-list').then((m) => m.RolesList),
      },
      {
        path: 'role-claims',
        canActivate: [permissionGuard],
        data: { permission: 'roles:view' },
        loadComponent: () =>
          import('./features/roles/pages/role-claims/role-claims').then((m) => m.RoleClaims),
      },
      {
        path: 'products/add',
        canActivate: [permissionGuard],
        data: { permission: 'products:create' },
        loadComponent: () =>
          import('./features/products/Pages/add-product/add-product').then((m) => m.AddProduct),
      },
      {
        path: 'product-histories',
        canActivate: [permissionGuard],
        data: { permission: 'product-status-histories:view' },
        loadComponent: () =>
          import('./features/productHistories/Pages/histories-list/status-histories-list/status-histories-list').then(
            (m) => m.StatusHistoriesList,
          ),
      },
      {
        path: 'products/:id',
        canActivate: [permissionGuard],
        data: { permission: 'products:view' },
        loadComponent: () =>
          import('./features/products/Pages/product-details/product-details').then(
            (m) => m.ProductDetails,
          ),
      },
    ],
  },
  {
    path: 'forbidden',
    component: Unauthorized,
  },
  {
    path: 'unauthorized',
    component: Unauthorized,
  },
  {
    path: 'login',
    loadComponent: () => import('./features/auth/Pages/Login/login').then((m) => m.Login),
    canActivate: [guestGuard],
  },
  {
    path: 'products',
    pathMatch: 'full',
    redirectTo: 'dashboard/products',
  },
  {
    path: 'products/add',
    pathMatch: 'full',
    redirectTo: 'dashboard/products/add',
  },
  {
    path: 'products/:id',
    pathMatch: 'full',
    redirectTo: 'dashboard/products/:id',
  },
  {
    path: 'add-product',
    pathMatch: 'full',
    redirectTo: 'dashboard/products/add',
  },
];
