# Frontend - Product Management System

A modern, responsive Angular application for managing products with role-based access control and beautiful user interface.

## 🚀 Overview

This frontend application provides an intuitive interface for the Product Management System, featuring real-time data synchronization, secure authentication, and a responsive design that works seamlessly across all devices.

## 🏗️ Architecture

The application follows **Angular best practices** with a modular, component-based architecture:

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                     │
│                   (Angular Components)                    │
└─────────────────────┬───────────────────────────────────┘
                      │
┌─────────────────────▼───────────────────────────────────┐
│                   Service Layer                         │
│              (HTTP Services, State Management)           │
└─────────────────────┬───────────────────────────────────┘
                      │
┌─────────────────────▼───────────────────────────────────┐
│                    Data Layer                          │
│                (Models, Interfaces)                     │
└─────────────────────────────────────────────────────────────┘
```

## 🛠️ Tech Stack

### Core Framework

- **Angular 21** - Modern TypeScript-based framework
- **TypeScript 5.9** - Type-safe JavaScript
- **RxJS 7.8** - Reactive programming
- **Angular Router** - Client-side routing

### UI & Styling

- **Tailwind CSS 4.2** - Utility-first CSS framework
- **PostCSS** - CSS post-processing
- **Angular SSR** - Server-side rendering support

### Third-Party Libraries

- **SweetAlert2** - Beautiful alert dialogs
- **ngx-toastr** - Toast notifications
- **jwt-decode** - JWT token parsing
- **Express 5** - SSR server (when enabled)

### Development Tools

- **Angular CLI 21** - Development tooling
- **Vitest** - Unit testing framework
- **ESLint** - Code linting
- **Prettier** - Code formatting

## 📁 Project Structure

```
FrontEnd/product-app/
├── src/
│   ├── app/                        # Main application module
│   │   ├── components/             # Reusable components
│   │   │   ├── auth/              # Authentication components
│   │   │   ├── products/          # Product components
│   │   │   ├── users/             # User management
│   │   │   └── shared/           # Shared UI components
│   │   ├── services/              # API services
│   │   │   ├── auth.service.ts    # Authentication service
│   │   │   ├── product.service.ts # Product API service
│   │   │   └── user.service.ts   # User API service
│   │   ├── models/               # TypeScript interfaces
│   │   │   ├── auth.models.ts    # Auth-related models
│   │   │   ├── product.models.ts # Product models
│   │   │   └── user.models.ts   # User models
│   │   ├── guards/               # Route guards
│   │   │   ├── auth.guard.ts     # Authentication guard
│   │   │   └── role.guard.ts    # Role-based guard
│   │   ├── interceptors/          # HTTP interceptors
│   │   │   └── auth.interceptor.ts # JWT token interceptor
│   │   ├── pages/                # Page components
│   │   │   ├── login/           # Login page
│   │   │   ├── dashboard/       # Dashboard
│   │   │   ├── products/        # Product pages
│   │   │   └── users/           # User management pages
│   │   ├── app-routing.module.ts # Application routing
│   │   ├── app.component.ts      # Root component
│   │   └── app.module.ts         # Root module
│   ├── assets/                   # Static assets
│   │   ├── images/              # Image files
│   │   └── styles/             # Global styles
│   ├── environments/             # Environment configurations
│   │   ├── environment.ts        # Development config
│   │   └── environment.prod.ts  # Production config
│   ├── styles/                   # Global styles
│   │   ├── globals.css          # Global Tailwind styles
│   │   └── main.css            # Main stylesheet
│   ├── index.html               # HTML entry point
│   ├── main.ts                 # Application bootstrap
│   └── polyfills.ts            # Browser polyfills
├── public/                      # Public assets
├── angular.json                 # Angular configuration
├── package.json                 # Dependencies and scripts
├── tsconfig.json               # TypeScript configuration
├── tailwind.config.js          # Tailwind configuration
└── README.md                   # This file
```

## ✨ Key Features

### 🔐 Authentication & Security

- JWT-based authentication with automatic token refresh
- Role-based access control (RBAC)
- Secure token storage with localStorage
- Route guards for protected pages
- Automatic logout on token expiration

### 📦 Product Management

- **Product Dashboard** - Overview with statistics
- **Product List** - Paginated, filterable product listing
- **Product Creation** - Form-based product creation
- **Product Details** - Detailed product view with history
- **Status Management** - Change product status with confirmation
- **Soft Delete** - Safe product deletion with undo option

### 👥 User Management

- **User Directory** - Browse and search users
- **User Creation** - Registration with role assignment
- **Profile Management** - View and edit user profiles
- **Role Assignment** - Assign roles and permissions

### 🎨 User Interface

- **Responsive Design** - Works on desktop, tablet, and mobile
- **Dark/Light Theme** - Theme switching capability
- **Loading States** - Skeleton loaders and spinners
- **Error Handling** - User-friendly error messages
- **Success Notifications** - Toast notifications for actions

### 📊 Data Visualization

- **Dashboard Analytics** - Product statistics and charts
- **Status Tracking** - Visual status indicators
- **Activity Feeds** - Recent activity timeline

## 🚀 Getting Started

### Prerequisites

- **Node.js 18+** and **npm**
- **Angular CLI 21** (`npm install -g @angular/cli`)
- **Modern web browser** (Chrome, Firefox, Safari, Edge)

### Installation

1. **Navigate to frontend directory**

   ```bash
   cd FrontEnd/product-app
   ```

2. **Install dependencies**

   ```bash
   npm install
   ```

3. **Configure API endpoint**
   Update `src/environments/environment.ts`:

   ```typescript
   export const environment = {
     production: false,
     apiUrl: "http://localhost:5119/api",
   };
   ```

4. **Start development server**

   ```bash
   ng serve
   ```

5. **Access application**
   Open `http://localhost:4200` in your browser

### Available Scripts

```bash
# Start development server
npm start
# or
ng serve

# Build for production
npm run build
# or
ng build

# Run tests
npm test
# or
ng test

# Build and watch for changes
npm run watch
# or
ng build --watch --configuration development

# Run with SSR (Server-Side Rendering)
npm run serve:ssr
```

## 🔧 Configuration

### Environment Variables

Create environment-specific configurations:

**Development** (`src/environments/environment.ts`):

```typescript
export const environment = {
  production: false,
  apiUrl: "http://localhost:5119/api",
  enableDebugMode: true,
  logLevel: "debug",
};
```

**Production** (`src/environments/environment.prod.ts`):

```typescript
export const environment = {
  production: true,
  apiUrl: "https://api.yourdomain.com/api",
  enableDebugMode: false,
  logLevel: "error",
};
```

### Tailwind CSS Configuration

Customize Tailwind in `tailwind.config.js`:

```javascript
/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./src/**/*.{html,ts}"],
  theme: {
    extend: {
      colors: {
        primary: {
          50: "#eff6ff",
          500: "#3b82f6",
          600: "#2563eb",
          700: "#1d4ed8",
        },
      },
    },
  },
  plugins: [],
};
```

## 🧪 Development

### Code Style

The project uses **Prettier** for code formatting:

```bash
# Format all files
npx prettier --write "src/**/*.{ts,html,css}"

# Check formatting
npx prettier --check "src/**/*.{ts,html,css}"
```

### Testing

```bash
# Run unit tests
ng test

# Run tests with coverage
ng test --code-coverage

# Run end-to-end tests
ng e2e
```

### Building

```bash
# Development build
ng build

# Production build
ng build --configuration production

# Build with SSR
ng build --configuration production && ng run product-app:server:production
```

## 🔐 Authentication Flow

### Login Process

1. User enters credentials on login page
2. Credentials sent to backend API
3. JWT token and refresh token received
4. Tokens stored securely in localStorage
5. User redirected to dashboard
6. Automatic token refresh on expiration

### Authorization

- **Route Guards**: Protect routes based on authentication status
- **Role Guards**: Restrict access based on user roles
- **HTTP Interceptor**: Automatically add JWT to API requests
- **Token Refresh**: Handle token expiration seamlessly

## 📱 Responsive Design

### Breakpoints

- **Mobile**: < 640px
- **Tablet**: 640px - 1024px
- **Desktop**: > 1024px

### Features

- **Mobile-First Design**: Optimized for mobile devices
- **Touch-Friendly**: Large tap targets and gestures
- **Adaptive Layout**: Components adjust to screen size
- **Progressive Enhancement**: Works without JavaScript

## 🚀 Deployment

### Build for Production

```bash
# Build optimized version
ng build --configuration production

# Output in: dist/product-app/
```

### Deployment Options

#### Static Hosting (Vercel, Netlify, GitHub Pages)

```bash
# Build and deploy dist/product-app/ folder
ng build --configuration production --base-href /your-repo-name/
```

#### Server-Side Rendering

```bash
# Build SSR version
ng build --configuration production && ng run product-app:server:production

# Deploy dist/product-app/server/ folder
```

#### Docker Deployment

```dockerfile
FROM node:18-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM node:18-alpine AS runtime
WORKDIR /app
COPY --from=build /app/dist/product-app ./dist
COPY package*.json ./
RUN npm ci --only=production
EXPOSE 4000
CMD ["node", "dist/product-app/server/main.js"]
```

### Environment Variables in Production

Set these environment variables for production:

```bash
export API_BASE_URL=https://api.yourdomain.com/api
export ENABLE_DEBUG_MODE=false
export LOG_LEVEL=error
```

## 🔧 Troubleshooting

### Common Issues

**CORS Errors**

- Ensure backend allows frontend origin
- Check CORS configuration in backend

**Authentication Issues**

- Clear browser localStorage
- Check JWT token expiration
- Verify API endpoint configuration

**Build Errors**

- Delete `node_modules` and run `npm install`
- Clear Angular cache: `ng cache clean`

**Performance Issues**

- Enable production builds
- Use lazy loading for large modules
- Optimize images and assets

### Debug Mode

Enable debug mode in development:

```typescript
// In app.component.ts
if (environment.enableDebugMode) {
  console.log("Debug mode enabled");
  // Add debug logging here
}
```

## 🤝 Contributing

1. **Follow Angular Style Guide**
2. **Write Tests** for new components and services
3. **Use TypeScript** strictly (no `any` types)
4. **Follow Git Conventional Commits**
5. **Update Documentation** for new features

### Git Workflow

```bash
# Create feature branch
git checkout -b feature/new-feature

# Make changes and commit
git add .
git commit -m "feat: add new feature"

# Push and create PR
git push origin feature/new-feature
```

## 📚 Learning Resources

- [Angular Documentation](https://angular.dev/overview)
- [Tailwind CSS Documentation](https://tailwindcss.com/docs)
- [RxJS Documentation](https://rxjs.dev/)
- [TypeScript Handbook](https://www.typescriptlang.org/docs/)

## 📝 License

This project is for educational and internship purposes.

---

**Built with ❤️ using Angular 21 and Tailwind CSS**
