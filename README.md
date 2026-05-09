# Product Management System

A full-stack web application for managing products with role-based access control, built with ASP.NET Core and Angular.

## Overview

This is a comprehensive product management system that demonstrates modern web development practices with a clean architecture, secure authentication, and responsive user interface.

## Architecture

The application follows a **layered architecture** with clear separation of concerns:

```
┌─────────────────┐    ┌─────────────────┐
│   Frontend      │    │    Backend      │
│   (Angular)     │◄──►│  (ASP.NET Core) │
│                 │    │                 │
│ - UI Components │    │ - Web API      │
│ - Services      │    │ - Business Logic│
│ - Routing       │    │ - Data Access  │
└─────────────────┘    └─────────────────┘
                              │
                       ┌─────────────────┐
                       │   Database      │
                       │  (SQL Server)   │
                       └─────────────────┘
```

## Tech Stack

### Frontend
- **Angular 21** - Modern TypeScript-based framework
- **Tailwind CSS** - Utility-first CSS framework
- **RxJS** - Reactive programming library
- **SweetAlert2** - Beautiful alert dialogs
- **ngx-toastr** - Toast notifications

### Backend
- **ASP.NET Core 8** - Web API framework
- **Entity Framework Core** - ORM for data access
- **ASP.NET Core Identity** - Authentication & authorization
- **JWT Authentication** - Secure token-based auth
- **MediatR** - CQRS pattern implementation
- **AutoMapper** - Object mapping
- **FluentValidation** - Input validation
- **Swagger/OpenAPI** - API documentation

### Database
- **SQL Server** - Relational database
- **Code-first migrations** - Schema management

## Key Features

### Security & Authentication
- JWT-based authentication with refresh tokens
- Role-based access control (RBAC)
- Permission-based authorization policies
- Secure password handling with ASP.NET Core Identity

### Product Management
- Create, read, update, delete products
- Product status tracking with history
- Soft delete functionality
- Advanced filtering and pagination

### User Management
- User registration and management
- Role assignment (ProjectManager, Supervisor, WarehouseManager)
- Granular permission control

### Analytics & Reporting
- Product statistics dashboard
- User activity tracking
- Status change history

## Quick Start

### Prerequisites
- **.NET 8 SDK** or later
- **Node.js 18+** and **npm**
- **SQL Server** (local or remote)
- **Angular CLI** (`npm install -g @angular/cli`)

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd DBS_Task
   ```

2. **Backend Setup**
   ```bash
   cd Backend
   dotnet restore
   dotnet run
   ```

3. **Frontend Setup**
   ```bash
   cd FrontEnd/product-app
   npm install
   ng serve
   ```

4. **Database Configuration**
   - Update connection string in `Backend/appsettings.json`
   - Run migrations: `dotnet ef database update`

### Access Points
- **Frontend**: `http://localhost:4200`
- **Backend API**: `http://localhost:5119`
- **API Documentation**: `http://localhost:5119/swagger`

## Project Structure

```
DBS_Task/
├── Backend/                 # ASP.NET Core Web API
│   ├── API/                # Controllers and endpoints
│   ├── Application/         # Business logic (CQRS)
│   ├── Domain/             # Core entities
│   ├── Infrastructure/      # Data access and services
│   └── Contracts/          # Request/response contracts
├── FrontEnd/
│   └── product-app/        # Angular application
│       ├── src/
│       │   ├── app/        # Application components
│       │   ├── components/ # Reusable UI components
│       │   ├── services/   # API services
│       │   └── models/     # TypeScript interfaces
│       └── public/         # Static assets
└── README.md              # This file
```

## Configuration

### Backend Configuration
Update `Backend/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=DBS_Task_Db;Trusted_Connection=True;"
  },
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-min-32-chars",
    "Issuer": "ProductManagement",
    "Audience": "ProductManagementUsers",
    "ExpirationMinutes": 60
  }
}
```

### Frontend Configuration
Update API base URL in `FrontEnd/product-app/src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5119/api'
};
```

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is for educational and internship purposes. Please add your preferred license before using in production.

## Support

For questions and support, please open an issue in the repository.

---

**Built with ❤️ for the DBS Internship Program**
