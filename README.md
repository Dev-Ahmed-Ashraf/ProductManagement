# DBS Task - Product Management API

A clean and scalable ASP.NET Core Web API for managing products, users, roles, and authorization with JWT.

This project follows a layered architecture with CQRS (MediatR), Entity Framework Core, and permission-based authorization.

## Tech Stack

- ASP.NET Core 8 Web API
- Entity Framework Core + SQL Server
- ASP.NET Core Identity
- JWT Authentication + Refresh Tokens
- MediatR (CQRS)
- AutoMapper
- FluentValidation
- Swagger / OpenAPI

## Project Structure

- `API` - controllers and HTTP layer
- `Application` - CQRS commands/queries, DTOs, validators, business use cases
- `Domain` - core entities
- `Infrastructure` - EF Core context, migrations, security, services, seeders
- `Contracts` - request contracts used by endpoints

## Features

- JWT login with refresh token flow
- Role and permission (claim) based authorization policies
- Product management:
  - Create product
  - List products (with filtering/pagination support via query contracts)
  - Get product by id with status history
  - Soft delete product
  - Change product status
- User management:
  - Create user
  - Get users (list/details)
- Roles and claims retrieval
- Statistics endpoint
- Swagger UI for testing endpoints

## Getting Started

### 1) Prerequisites

- .NET SDK 8.0+
- SQL Server (local or remote)

### 2) Clone and Restore

```bash
git clone <your-repository-url>
cd DBS_Task
dotnet restore
```

### 3) Configure Settings

Update `appsettings.json`:

- `ConnectionStrings:DefaultConnection` with your SQL Server connection string
- `JwtSettings:SecretKey` with a strong secret key (or provide it via environment variable)

Example environment variable (PowerShell):

```powershell
$env:JwtSettings__SecretKey="your-very-strong-secret-key"
```

### 4) Apply Database Migrations

```bash
dotnet ef database update
```

If `dotnet ef` is missing:

```bash
dotnet tool install --global dotnet-ef
```

### 5) Run the API

```bash
dotnet run
```

Swagger will be available at:

- `http://localhost:5119/swagger`
- or the HTTPS URL from `Properties/launchSettings.json`

## Authentication and Authorization

1. Call `POST /api/Auth/login` to get an access token.
2. In Swagger, click **Authorize** and enter:

```text
Bearer <your_access_token>
```

3. Access protected endpoints based on your role claims (`permission` claims).

## Main API Endpoints

- `POST /api/Auth/login`
- `POST /api/Auth/refresh-token`
- `POST /api/Auth/logout`
- `GET /api/Products`
- `POST /api/Products`
- `GET /api/Products/{id}`
- `PATCH /api/Products/{id}/status`
- `DELETE /api/Products/{id}`
- `GET /api/ProductStatusHistories`
- `GET /api/Users`
- `POST /api/Users`
- `GET /api/Users/{id}`
- `GET /api/Roles`
- `GET /api/Roles/{id}/claims`
- `GET /api/Statistics`

## Notes

- Roles and permission claims are seeded in the database at startup.
- Protected routes require valid JWT tokens and matching authorization policies.

## License

This project is for internship/training purposes.  
You can add your preferred license (for example, MIT) before publishing publicly.

