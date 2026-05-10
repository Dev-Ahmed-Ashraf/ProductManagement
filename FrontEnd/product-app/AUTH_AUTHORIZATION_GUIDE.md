# Authentication & Authorization Flow Guide

## 📋 Table of Contents

1. [System Overview](#system-overview)
2. [Authentication Guards](#authentication-guards)
3. [Permission System](#permission-system)
4. [HasPermission Directive](#haspermission-directive)
5. [End-to-End Flow](#end-to-end-flow)
6. [Real-World Examples](#real-world-examples)

---

## System Overview

Your application uses a **permission-based authorization system** with:

- **JWT Tokens** for authentication
- **Role-Based Access Control (RBAC)** for authorization
- **4 Guards** to protect routes at different levels
- **1 Directive** to show/hide UI elements based on permissions
- **1 Service** (AuthService) to manage authentication state

---

## 🛡️ Authentication Guards

### 1. **authGuard** - Basic Authentication Check

**File:** [src/app/core/guards/auth.guard.ts](src/app/core/guards/auth.guard.ts)

**Purpose:** Protects routes that require user to be logged in.

```typescript
// ✅ ALLOWS: Logged-in users
// ❌ BLOCKS: Non-logged-in users → Redirects to /login
```

**Used In:**

```typescript
{
  path: 'dashboard',
  canActivate: [authGuard],  // Must be logged in
  ...
}
```

**Flow:**

```
User Access Route?
    ↓
authGuard runs
    ↓
Is Access Token Valid & Not Expired?
    ├─ YES → Allow Access ✅
    └─ NO → Redirect to /login ❌
```

---

### 2. **guestGuard** - Opposite of authGuard

**File:** [src/app/core/guards/guest.guard.ts](src/app/core/guards/guest.guard.ts)

**Purpose:** Protects routes meant for unauthenticated users (e.g., login page).

```typescript
// ✅ ALLOWS: Non-logged-in users
// ❌ BLOCKS: Logged-in users → Redirects to /dashboard
```

**Used In:**

```typescript
{
  path: 'login',
  canActivate: [guestGuard],  // Only if NOT logged in
  ...
}
```

**Why?** Prevent users from accessing login page if already logged in.

---

### 3. **permissionGuard** - Fine-Grained Permission Control

**File:** [src/app/core/guards/permission.guard.ts](src/app/core/guards/permission.guard.ts)

**Purpose:** Checks if user has SPECIFIC permissions before allowing route access.

```typescript
// ✅ ALLOWS: User with required permission(s)
// ❌ BLOCKS: User without permission → Redirects to /forbidden
```

**Configuration Options:**

#### Mode: 'all' (DEFAULT) - User needs ALL permissions

```typescript
{
  path: 'users/add',
  canActivate: [permissionGuard],
  data: {
    permission: ['users:create', 'users:view'],  // Array
    mode: 'all'  // Optional, default is 'all'
  }
}
// User MUST have BOTH permissions
```

#### Mode: 'any' - User needs AT LEAST ONE permission

```typescript
{
  path: 'reports',
  canActivate: [permissionGuard],
  data: {
    permission: ['reports:view', 'reports:admin'],  // Array
    mode: 'any'
  }
}
// User needs at least 1 of these permissions
```

#### Single Permission

```typescript
{
  path: 'products',
  canActivate: [permissionGuard],
  data: { permission: 'products:view' }  // String - automatically 'all' mode
}
// User needs exactly this permission
```

**Used In Routes:**

```typescript
{
  path: 'products',
  canActivate: [permissionGuard],
  data: { permission: 'products:view' },
  ...
},
{
  path: 'users/add',
  canActivate: [permissionGuard],
  data: { permission: 'users:create' },
  ...
},
{
  path: 'roles',
  canActivate: [permissionGuard],
  data: { permission: 'roles:view' },
  ...
}
```

---

### 4. **dashboardLandingGuard** - Smart Route Redirection

**File:** [src/app/core/guards/dashboard-landing.guard.ts](src/app/core/guards/dashboard-landing.guard.ts)

**Purpose:** When user navigates to `/dashboard` (without specific child route), redirect them to the FIRST page they have permission to access.

**Pre-configured Landing Routes:**

```typescript
const LANDING_ROUTES = [
  { route: '/dashboard/statistics', permission: 'statistics:view' },
  { route: '/dashboard/products', permission: 'products:view' },
  { route: '/dashboard/product-histories', permission: 'product-status-histories:view' },
  { route: '/dashboard/products/add', permission: 'products:create' },
  { route: '/dashboard/users', permission: 'users:view' },
  { route: '/dashboard/users/add', permission: 'users:create' },
  { route: '/dashboard/roles', permission: 'roles:view' },
];
```

**Flow:**

```
User navigates to /dashboard
    ↓
dashboardLandingGuard runs
    ↓
Loop through LANDING_ROUTES in order
    ↓
Find first route where user has permission
    ↓
Redirect to that route
    ↓
If NO permissions for any route → Redirect to /forbidden
```

**Example Scenarios:**

```
Scenario 1: User with 'products:view' permission
/dashboard → /dashboard/products ✅

Scenario 2: User with 'statistics:view' permission only
/dashboard → /dashboard/statistics ✅

Scenario 3: User with NO permissions
/dashboard → /forbidden ❌

Scenario 4: User with multiple permissions (products + users)
/dashboard → /dashboard/statistics (first in list they have access to)
```

---

## 🔑 Permission System

### How Permissions Are Stored

**1. In JWT Token:**

```json
{
  "sub": "user-123",
  "email": "user@example.com",
  "fullName": "John Doe",
  "role": "Admin",
  "permission_json": "[\"products:view\",\"products:create\",\"users:view\",\"roles:view\"]",
  "exp": 1234567890
}
```

**2. In Browser Storage:**

```javascript
// localStorage or sessionStorage
{
  "session": {
    "accessToken": "eyJhbGc...",
    "refreshToken": "eyJhbGc...",
    "fullName": "John Doe",
    "roles": ["Admin"]
  },
  "accessToken": "eyJhbGc...",
  "refreshToken": "eyJhbGc..."
}
```

### Permission Extraction Process

**Step-by-Step:**

```typescript
1. User logs in
   ↓
2. AuthService.setSession(response, rememberMe)
   ├─ Stores tokens in localStorage/sessionStorage
   └─ Calls extractPermissions()
   ↓
3. extractPermissions() runs
   ├─ Tries extractPermissionsFromToken()
   │  ├─ Decodes JWT
   │  ├─ Gets 'permission_json' claim
   │  ├─ JSON.parse() to array
   │  └─ Convert to Set with .toLowerCase()
   │
   └─ If token has no permissions, fallback to stored session
   ↓
4. Update permissionsSubject with new Set
   ↓
5. permissions$ Observable emits new value
   ↓
6. All subscribers (Directive, Components) get notified
```

**Code:**

```typescript
private extractPermissionsFromToken(): Set<string> {
  const claims = this.decodeToken();
  if (!claims) return new Set();

  try {
    const raw = claims.permission_json;
    if (!raw) return new Set();

    const parsed: string[] = JSON.parse(raw);
    return new Set(parsed.map((p) => p.toLowerCase()));
  } catch {
    return new Set();
  }
}
```

### Permission Methods in AuthService

```typescript
// Check if user has specific permission
hasPermission(permission: string): boolean
// Returns: true/false (case-insensitive)
// Usage: auth.hasPermission('users:view')

// Get all user permissions
getPermissions(): string[]
// Returns: Array of all permissions
// Usage: auth.getPermissions()

// Observable for reactive updates
permissions$: Observable<Set<string>>
// Emits whenever permissions change
// Usage: auth.permissions$.subscribe(...)
```

---

## 🎯 HasPermission Directive

**File:** [src/app/core/directives/has-permission.directive.ts](src/app/core/directives/has-permission-directive.ts)

**Purpose:** Show/hide HTML elements based on user permissions.

### Syntax

#### Single Permission

```html
<div *hasPermission="'users:view'">Users Section (Only for users with 'users:view' permission)</div>
```

#### Multiple Permissions (ALL required)

```html
<div *hasPermission="['users:view', 'users:create']">
  Users Management (Only if user has BOTH permissions)
</div>
```

#### Multiple Permissions with \*ngIf Alternative

```html
<div *ngIf="auth.hasPermission('users:view')">Alternative: Using component method directly</div>
```

### Real-World Example

```html
<!-- Navigation Menu -->
<nav>
  <a routerLink="/dashboard">Dashboard</a>

  <div *hasPermission="'products:view'">
    <a routerLink="/dashboard/products">Products</a>
  </div>

  <div *hasPermission="'users:view'">
    <a routerLink="/dashboard/users">Users</a>
  </div>

  <div *hasPermission="'roles:view'">
    <a routerLink="/dashboard/roles">Roles</a>
  </div>
</nav>

<!-- Page Content -->
<section>
  <h1>Product Management</h1>

  <!-- View/List Products -->
  <div *hasPermission="'products:view'">
    <product-list></product-list>
  </div>

  <!-- Create New Product Button -->
  <button *hasPermission="'products:create'" (click)="openAddProduct()">Add Product</button>

  <!-- Delete Product Button -->
  <button *hasPermission="'products:delete'" (click)="deleteProduct()">Delete</button>
</section>
```

### How It Works

```typescript
// Directive subscribes to permissions$ Observable
constructor(...) {
  this.auth.permissions$
    .pipe(takeUntil(this.destroy$))
    .subscribe(() => this.updateView());
}

// When @Input changes or permissions update
@Input('hasPermission')
set required(value: string | string[]) {
  this.requiredPermissions = Array.isArray(value) ? value : [value];
  this.updateView();
}

// Check if user has ALL required permissions
private check(): boolean {
  return this.requiredPermissions.every((perm) =>
    this.auth.hasPermission(perm)
  );
}

// Show/hide element
private updateView() {
  if (this.check() && !this.hasView) {
    this.vcr.createEmbeddedView(this.template);  // Insert in DOM
    this.hasView = true;
  } else if (!this.check() && this.hasView) {
    this.vcr.clear();  // Remove from DOM
    this.hasView = false;
  }
}
```

**Key Features:**

- ✅ **Reactive:** Updates when permissions change (token refresh)
- ✅ **Clean DOM:** Removes elements when permission denied (not just hidden)
- ✅ **Memory Safe:** Unsubscribes in ngOnDestroy
- ✅ **Case-Insensitive:** `'Users:View'` = `'users:view'`

---

## 🔄 End-to-End Flow

### Complete User Journey

```
┌─────────────────────────────────────────────────────────┐
│                    USER VISITS APP                      │
└──────────────────────┬──────────────────────────────────┘
                       ↓
        ┌──────────────────────────────┐
        │ App Initialization           │
        │ app.ts: initAuth() called    │
        └──────────────────┬───────────┘
                           ↓
        ┌──────────────────────────────────────┐
        │ Check for Token in Storage           │
        │ getAccessToken()                     │
        └──────────────────┬───────────────────┘
                           ↓
                    ┌──────────┐
                    │Has Token?│
                    └─────┬────┘
            ┌─────────────┼─────────────┐
            ↓             ↓             ↓
         YES            NO         EXPIRED
         ↓              ↓              ↓
    Extract Perms   Stay on         Clear
    Create User      Login          Session
    Object           Page           Try
    permissions$                   Refresh
    emits            ↓
                  Show Login Form
    ↓
    Router checks route guards
    ├─ authGuard: User logged in? ✅
    └─ permissionGuard: Has permissions? ✅

    ↓
    Route activated
    Component loads

    ↓
    Component checks for:
    ├─ auth.hasPermission() → true/false
    ├─ auth.getPermissions() → array
    └─ *hasPermission directive → show/hide

    ↓
    UI displays only actions user can perform

    ↓
    User performs action

    ↓
    HTTP Interceptor attaches token to request

    ↓
    If token expires during action:
    ├─ authHttpService.refreshToken() called
    ├─ New token obtained
    ├─ extractPermissions() called again
    ├─ permissions$ emits new value
    ├─ All directives & components update
    └─ Original request retried

    ↓
    User Logout:
    ├─ clearSession() called
    ├─ All storage cleared
    ├─ permissionsSubject → empty Set
    ├─ currentUser → null
    ├─ permissions$ emits empty Set
    ├─ All UI elements with *hasPermission hidden
    └─ Router redirects to /login
```

---

## 💡 Real-World Examples

### Example 1: Admin User

```
Token contains:
  permission_json: [
    "products:view",
    "products:create",
    "products:update",
    "products:delete",
    "users:view",
    "users:create",
    "users:update",
    "users:delete",
    "roles:view",
    "roles:create",
    "statistics:view"
  ]

Routes they can access:
  ✅ /dashboard/products
  ✅ /dashboard/products/add
  ✅ /dashboard/users
  ✅ /dashboard/users/add
  ✅ /dashboard/roles
  ✅ /dashboard/statistics

Dashboard nav shows all menu items
```

### Example 2: Product Manager

```
Token contains:
  permission_json: [
    "products:view",
    "products:create",
    "products:update",
    "products:delete",
    "product-status-histories:view"
  ]

Routes they can access:
  ✅ /dashboard/products (via permissionGuard)
  ✅ /dashboard/products/add (via permissionGuard)
  ✅ /dashboard/product-histories (via permissionGuard)
  ❌ /dashboard/users (no permission)
  ❌ /dashboard/roles (no permission)

Dashboard nav shows only product-related items

When they go to /dashboard:
  → dashboardLandingGuard checks LANDING_ROUTES
  → First allowed route is /dashboard/products
  → Redirected to /dashboard/products ✅
```

### Example 3: View-Only User

```
Token contains:
  permission_json: [
    "products:view",
    "users:view",
    "roles:view"
  ]

Routes they can access:
  ✅ /dashboard/products (can view)
  ❌ /dashboard/products/add (no create permission)
  ✅ /dashboard/users (can view)
  ❌ /dashboard/users/add (no create permission)
  ✅ /dashboard/roles (can view)

Template shows read-only buttons:

<div *hasPermission="'products:view'">
  <table><!-- Products list --></table>
</div>

<button *hasPermission="'products:create'" (click)="add()">
  Add Product  <!-- HIDDEN - user can't see this -->
</button>

<button *hasPermission="'products:delete'" (click)="delete()">
  Delete  <!-- HIDDEN - user can't see this -->
</button>
```

### Example 4: Component Implementation

```typescript
import { Component, inject } from '@angular/core';
import { AuthService } from '@core/services/auth/auth';

@Component({
  selector: 'app-product-list',
  template: `
    <h1>Products</h1>

    <!-- Action bar -->
    <div class="actions">
      <!-- Add button: Only visible if user can create -->
      <button *hasPermission="'products:create'" (click)="openAddProduct()">Add Product</button>

      <!-- Export button: Only visible if user has permission -->
      <button *hasPermission="'products:export'" (click)="exportProducts()">Export</button>
    </div>

    <!-- Product list: Only visible if user can view -->
    <div *hasPermission="'products:view'">
      <table>
        <tr *ngFor="let product of products">
          <td>{{ product.name }}</td>
          <td>{{ product.price }}</td>
          <td>
            <!-- Edit button -->
            <button *hasPermission="'products:update'" (click)="editProduct(product)">Edit</button>

            <!-- Delete button -->
            <button *hasPermission="'products:delete'" (click)="deleteProduct(product)">
              Delete
            </button>
          </td>
        </tr>
      </table>
    </div>
  `,
})
export class ProductListComponent {
  private auth = inject(AuthService);

  products: any[] = [];

  constructor() {
    this.loadProducts();
  }

  loadProducts() {
    // Only load if user has permission
    if (!this.auth.hasPermission('products:view')) {
      return;
    }
    // Load products...
  }

  openAddProduct() {
    // Double-check permission before action
    if (!this.auth.hasPermission('products:create')) {
      return;
    }
    // Open modal...
  }

  editProduct(product: any) {
    if (!this.auth.hasPermission('products:update')) {
      return;
    }
    // Edit logic...
  }

  deleteProduct(product: any) {
    if (!this.auth.hasPermission('products:delete')) {
      return;
    }
    // Delete logic...
  }

  exportProducts() {
    if (!this.auth.hasPermission('products:export')) {
      return;
    }
    // Export logic...
  }
}
```

---

## 🚀 Security Best Practices

### 1. **Always Use Guards on Routes**

```typescript
// ✅ GOOD
{
  path: 'users/add',
  canActivate: [authGuard, permissionGuard],
  data: { permission: 'users:create' }
}

// ❌ BAD - No protection
{
  path: 'users/add',
  component: AddUserComponent
}
```

### 2. **Double-Check in Component Methods**

```typescript
// ✅ GOOD - Check permission before action
deleteProduct() {
  if (!this.auth.hasPermission('products:delete')) {
    return;
  }
  // Delete...
}

// ⚠️ RISKY - Only frontend check, server must validate too
deleteProduct() {
  // Delete... (Backend should validate token!)
}
```

### 3. **Use Directive for UI Elements**

```typescript
// ✅ GOOD - Hides button from UI
<button *hasPermission="'users:delete'">Delete</button>

// ⚠️ RISKY - Disabled but visible
<button [disabled]="!canDelete">Delete</button>

// ❌ WRONG - User might bypass with DevTools
<button *ngIf="canDelete">Delete</button>  <!-- Can enable in console -->
```

### 4. **Backend Validation is Critical**

```typescript
// Frontend protects UX, Backend protects DATA
// Even if user bypasses all frontend checks,
// Backend MUST validate the JWT and permissions on every request
```

---

## 📊 Permission Format Convention

Recommended permission naming:

```
[feature]:[action]

Examples:
- products:view          // View/list products
- products:create        // Create new product
- products:update        // Edit product
- products:delete        // Delete product
- users:view             // View users
- users:create           // Create users
- roles:view             // View roles
- statistics:view        // View statistics
- product-status-histories:view
```

---

## 🎓 Summary Table

| Component                   | Purpose                               | How It Works                                                     | Usage                                                            |
| --------------------------- | ------------------------------------- | ---------------------------------------------------------------- | ---------------------------------------------------------------- |
| **authGuard**               | Basic login check                     | Checks `isLoggedIn()`                                            | `canActivate: [authGuard]`                                       |
| **guestGuard**              | Prevent logged-in users on login page | Inverts `isLoggedIn()` check                                     | `canActivate: [guestGuard]`                                      |
| **permissionGuard**         | Fine-grained permission check         | Compares user permissions with route requirements                | `canActivate: [permissionGuard]` + `data: { permission: '...' }` |
| **dashboardLandingGuard**   | Smart redirect to first allowed page  | Loops through predefined routes, finds first one user can access | `canActivate: [dashboardLandingGuard]` on `/dashboard`           |
| **HasPermission Directive** | Show/hide UI elements                 | Checks permission, adds/removes from DOM                         | `*hasPermission="'perm:action'"`                                 |
| **AuthService**             | Central auth management               | Stores tokens, extracts permissions, manages login/logout        | Injected in guards & components                                  |

---

## 🔗 File References

- **Directives:** [src/app/core/directives/has-permission.directive.ts](src/app/core/directives/has-permission.directive.ts)
- **Guards:** [src/app/core/guards/](src/app/core/guards/)
- **Service:** [src/app/core/services/auth/auth.ts](src/app/core/services/auth/auth.ts)
- **Routes:** [src/app/app.routes.ts](src/app/app.routes.ts)
