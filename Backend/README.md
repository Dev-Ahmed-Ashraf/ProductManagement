# Backend API - Product Management System

A robust ASP.NET Core Web API built with clean architecture, CQRS pattern, and enterprise-grade security features.

## 🚀 Overview

This backend provides a complete RESTful API for product management with role-based access control, JWT authentication, and comprehensive business logic implementation.

## 🏗️ Architecture

The backend follows **Clean Architecture** principles with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                     │
│                   (API Controllers)                      │
└─────────────────────┬───────────────────────────────────┘
                      │
┌─────────────────────▼───────────────────────────────────┐
│                   Application Layer                     │
│              (CQRS, DTOs, Validation)                 │
└─────────────────────┬───────────────────────────────────┘
                      │
┌─────────────────────▼───────────────────────────────────┐
│                     Domain Layer                       │
│                (Entities, Interfaces)                   │
└─────────────────────┬───────────────────────────────────┘
                      │
┌─────────────────────▼───────────────────────────────────┐
│                Infrastructure Layer                      │
│        (Data Access, External Services, Security)       │
└─────────────────────────────────────────────────────────────┘
```

## 🛠️ Tech Stack

### Core Framework
- **ASP.NET Core 8** - Modern web framework
- **Entity Framework Core 8** - ORM and data access
- **ASP.NET Core Identity** - Authentication & user management
- **SQL Server** - Primary database

### Architecture & Patterns
- **MediatR** - CQRS (Command Query Responsibility Segregation)
- **AutoMapper** - Object-to-object mapping
- **FluentValidation** - Input validation
- **Repository Pattern** - Data access abstraction

### Security & Authentication
- **JWT Bearer Authentication** - Token-based security
- **Refresh Tokens** - Secure session management
- **Role-Based Authorization** - RBAC implementation
- **Permission-Based Policies** - Granular access control

### Documentation & Testing
- **Swagger/OpenAPI** - Interactive API documentation
- **Health Checks** - Application monitoring
- **Logging** - Structured logging with built-in providers

## 📁 Project Structure

```
Backend/
├── API/                           # Presentation Layer
│   ├── Controllers/               # API endpoints
│   │   ├── AuthController.cs      # Authentication endpoints
│   │   ├── ProductController.cs   # Product management
│   │   ├── UsersController.cs     # User management
│   │   └── RolesController.cs     # Role management
│   └── DebugController.cs         # Debugging utilities
├── Application/                    # Application Layer
│   ├── Common/                    # Shared components
│   │   ├── Behaviors/            # MediatR pipeline behaviors
│   │   ├── Constants/            # Application constants
│   │   ├── Interfaces/           # Service interfaces
│   │   └── Results/             # Response wrappers
│   ├── Auth/                     # Authentication logic
│   ├── DTOs/                     # Data transfer objects
│   ├── Mappings/                 # AutoMapper profiles
│   ├── Products/                 # Product domain logic
│   ├── ProductStatusHistories/    # Status tracking
│   └── Users/                    # User management logic
├── Domain/                       # Domain Layer
│   ├── Common/                   # Shared domain entities
│   └── Entities/                 # Core business entities
├── Infrastructure/                # Infrastructure Layer
│   ├── Data/                     # Data access
│   │   ├── DBContext/           # EF Core context
│   │   ├── Interceptors/        # EF Core interceptors
│   │   └── Seeders/             # Database seeding
│   ├── Security/                 # Security implementations
│   │   ├── JwtSettings.cs       # JWT configuration
│   │   └── PermissionAuthorizationHandler.cs
│   ├── Services/                 # External services
│   └── Extensions/               # Service extensions
├── Contracts/                    # Request/Response contracts
├── Program.cs                   # Application entry point
├── appsettings.json             # Configuration
└── DBS_Task.csproj             # Project file
```

## 🔐 Security Features

### Authentication Flow
1. **User Login** - Validates credentials and returns JWT + refresh token
2. **Token Validation** - Middleware validates JWT on each request
3. **Refresh Token** - Secure token renewal without re-authentication
4. **Authorization** - Role and permission-based access control

### Authorization Model
- **Roles**: ProjectManager, Supervisor, WarehouseManager
- **Permissions**: Granular permissions (products:view, products:create, etc.)
- **Policies**: Authorization policies based on permission claims

### Security Best Practices
- Password hashing with ASP.NET Core Identity
- JWT token expiration and refresh mechanism
- Secure token storage recommendations
- CORS configuration for cross-origin requests

## 📊 API Endpoints

### Authentication
- `POST /api/Auth/login` - User authentication
- `POST /api/Auth/refresh-token` - Token refresh
- `POST /api/Auth/logout` - User logout

### Products
- `GET /api/Products` - List products with pagination/filtering
- `POST /api/Products` - Create new product
- `GET /api/Products/{id}` - Get product by ID
- `PATCH /api/Products/{id}/status` - Change product status
- `DELETE /api/Products/{id}` - Soft delete product

### Users
- `GET /api/Users` - List users
- `POST /api/Users` - Create user
- `GET /api/Users/{id}` - Get user by ID

### Roles & Permissions
- `GET /api/Roles` - List all roles
- `GET /api/Roles/{id}/claims` - Get role permissions

### Statistics
- `GET /api/Statistics` - Application statistics

## 🚀 Getting Started

### Prerequisites
- **.NET 8 SDK** or later
- **SQL Server** (local or remote)
- **Visual Studio 2022** or **VS Code**

### Installation

1. **Clone repository**
   ```bash
   git clone <repository-url>
   cd DBS_Task/Backend
   ```

2. **Install dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure database**
   - Update `appsettings.json` with your SQL Server connection string
   - Install EF Core tools if not installed:
     ```bash
     dotnet tool install --global dotnet-ef
     ```

4. **Apply migrations**
   ```bash
   dotnet ef database update
   ```

5. **Run the application**
   ```bash
   dotnet run
   ```

### Configuration

Update `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=DBS_Task_Db;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-minimum-32-characters",
    "Issuer": "ProductManagement",
    "Audience": "ProductManagementUsers",
    "ExpirationMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Environment Variables
For production, use environment variables:

```bash
# PowerShell
$env:ConnectionStrings__DefaultConnection="Server=prod-server;Database=DBS_Task_Db;..."
$env:JwtSettings__SecretKey="your-production-secret-key"

# Bash/Linux
export ConnectionStrings__DefaultConnection="Server=prod-server;Database=DBS_Task_Db;..."
export JwtSettings__SecretKey="your-production-secret-key"
```

## 🧪 Development

### Running Tests
```bash
dotnet test
```

### Adding New Features
1. **Domain**: Add entities in `Domain/Entities/`
2. **Application**: Add CQRS handlers in `Application/`
3. **API**: Add controllers in `API/Controllers/`
4. **Infrastructure**: Add services in `Infrastructure/`

### Database Migrations
```bash
# Add new migration
dotnet ef migrations add MigrationName

# Apply migration
dotnet ef database update

# Generate SQL script
dotnet ef migrations script
```

## 📝 API Documentation

Once running, access the interactive API documentation at:
- **Swagger UI**: `http://localhost:5119/swagger`
- **OpenAPI JSON**: `http://localhost:5119/swagger/v1/swagger.json`

## 🔧 Debugging

### Debug Endpoints
- `GET /api/debug/claims` - View current user claims
- `GET /api/debug/protected` - Test authentication
- `GET /api/debug/protected-permission` - Test authorization

### Logging
The application includes comprehensive logging. Check console output for:
- Authentication events
- Authorization failures
- Database operations
- Application errors

## 🚀 Deployment

### Docker Deployment
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DBS_Task.csproj", "./"]
RUN dotnet restore "./DBS_Task.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "DBS_Task.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DBS_Task.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DBS_Task.dll"]
```

### Production Considerations
- Use HTTPS in production
- Configure proper CORS policies
- Set up proper logging and monitoring
- Use environment variables for sensitive data
- Implement rate limiting and throttling
- Set up database backups

## 🤝 Contributing

1. Follow the existing code style and patterns
2. Write unit tests for new features
3. Update API documentation
4. Ensure all tests pass before submitting PR

## 📝 License

This project is for educational and internship purposes.

---

**Built with ❤️ using ASP.NET Core 8**
