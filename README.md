# SimpleNotesApp Backend

A clean architecture REST API for a note-taking application with JWT authentication and passwordless OTP login.

**Android Client**: [simple-notes-android](https://github.com/olekslukian/simple-notes-android)

## Features

- 📝 **CRUD Operations** - Create, read, update, and delete notes
- 🔐 **JWT Authentication** - Secure token-based authentication with refresh tokens
- 📧 **Passwordless Login** - OTP-based authentication via email (Mailgun)
- 🔒 **Password-based Login** - Traditional email/password authentication
- 🧪 **Comprehensive Testing** - Unit tests for business logic and API controllers

## Tech Stack

- **.NET 8.0** - Web API framework
- **Dapper 2.1.35** - Lightweight ORM for SQL Server stored procedures
- **Microsoft SQL Server** - Database with stored procedures
- **JWT Bearer** - Token-based authentication
- **Mailgun** - Email service for OTP delivery
- **Swagger/OpenAPI** - Interactive API documentation
- **xUnit + Moq + FluentAssertions** - Testing framework

## Architecture

Clean Architecture with dependency inversion:

```
src/
├── SimpleNotesApp.API/           # Controllers, middleware, dependency injection
├── SimpleNotesApp.Core/          # Business logic, services, DTOs, interfaces
└── SimpleNotesApp.Infrastructure/ # Data access, Dapper repositories, external services

tests/
├── SimpleNotesApp.Core.Tests/    # Unit tests for services and helpers
└── SimpleNotesApp.API.Tests/     # Unit tests for API controllers
```

**Dependency Flow**: `API → Core ← Infrastructure`

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server)

### 1. Clone & Setup

```bash
git clone https://github.com/olekslukian/simple-notes-backend.git
cd simple-notes-backend
```

### 2. Configure Secrets (Development)

```bash
cd src/SimpleNotesApp.API
dotnet user-secrets init

# Database connection
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=NotesAppDb;Trusted_Connection=true;TrustServerCertificate=true;"

# JWT and password encryption (generate secure random strings)
dotnet user-secrets set "AppSettings:TokenKey" "your-super-secret-jwt-key-at-least-32-characters-long"
dotnet user-secrets set "AppSettings:PasswordKey" "your-password-encryption-key-at-least-32-chars"

# Mailgun configuration (for OTP emails)
dotnet user-secrets set "MailgunSettings:ApiKey" "your-mailgun-api-key"
dotnet user-secrets set "MailgunSettings:Domain" "your-domain.mailgun.org"
dotnet user-secrets set "MailgunSettings:FromEmail" "noreply@yourdomain.com"
dotnet user-secrets set "MailgunSettings:FromName" "Simple Notes App"
```

### 3. Database Setup

1. Create database: `NotesAppDb`
2. Execute SQL scripts from `Stored Procedures/` directory in order:
   - `spRegistration_Upsert.sql` - User registration
   - `spCheck_User.sql` - Email verification
   - `spUser_Auth_Confirmation.sql` - Password authentication
   - `spUser_getById.sql`, `spUserId_get.sql` - User lookup
   - `spUser_getByRefToken.sql`, `spUser_RefreshToken_Update.sql` - Token management
   - `spNote_*.sql` - Notes CRUD operations

### 4. Run the Application

```bash
# From the solution root
dotnet run --project src/SimpleNotesApp.API
```

- **HTTPS**: `https://localhost:7108`
- **HTTP**: `http://localhost:5108`
- **Swagger UI**: `https://localhost:7108/swagger` 📖

## API Endpoints

### Authentication
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| `POST` | `/api/auth/register` | Register new user with email/password | ❌ |
| `POST` | `/api/auth/login` | Login with email/password | ❌ |
| `POST` | `/api/auth/send-otp-login` | Request OTP for passwordless login | ❌ |
| `POST` | `/api/auth/verify-email-login` | Verify email and get OTP confirmation | ❌ |
| `POST` | `/api/auth/send-otp-password` | Request OTP to set password | ✅ |
| `POST` | `/api/auth/set-password` | Set password using OTP | ✅ |
| `POST` | `/api/auth/change-password` | Change password (requires old password) | ✅ |
| `POST` | `/api/auth/refresh-token` | Refresh JWT using refresh token | ❌ |

### Notes
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| `GET` | `/api/notes` | Get all user notes | ✅ |
| `GET` | `/api/notes/{id}` | Get specific note by ID | ✅ |
| `POST` | `/api/notes` | Create new note | ✅ |
| `PATCH` | `/api/notes/{id}` | Update note | ✅ |
| `DELETE` | `/api/notes/{id}` | Delete note | ✅ |

## Quick Test

```bash
# Register a new user
curl -X POST https://localhost:7108/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test123!",
    "passwordConfirmation": "Test123!"
  }'

# Login
curl -X POST https://localhost:7108/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test123!"
  }'
```

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/SimpleNotesApp.Core.Tests
dotnet test tests/SimpleNotesApp.API.Tests

# Run with detailed output
dotnet test --verbosity normal
```

**Test Coverage**:
- ✅ **Core Tests** (44 tests) - AuthService, NotesService, AuthHelper
- ✅ **API Tests** (24 tests) - AuthController, NotesController

## 🏗️ Development

```bash
# Build the solution
dotnet build

# Run with hot reload
dotnet watch run --project src/SimpleNotesApp.API

# View Swagger documentation
open https://localhost:7108/swagger
```



