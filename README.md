# SimpleNotesApp Backend

A clean architecture REST API for a note-taking application with JWT authentication.
Client App is implemented [here](https://github.com/olekslukian/simple-notes-android).

## Tech Stack

- **.NET 8.0** - Web API framework
- **Dapper** - Lightweight ORM for data access
- **Microsoft SQL Server** - Database
- **JWT Bearer Authentication** - Secure token-based auth
- **Swagger/OpenAPI** - API documentation
- **Clean Architecture** - Separation of concerns

##  Project Structure

```
src/
├── SimpleNotesApp.API/           # Controllers, Program.cs, Swagger
├── SimpleNotesApp.Core/          # Business logic, DTOs, Interfaces
└── SimpleNotesApp.Infrastructure/ # Data access, Models, SQL procedures
```

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

# Set your database connection
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=NotesAppDb;Trusted_Connection=true;TrustServerCertificate=true;"

# Generate secure keys (replace with your own)
dotnet user-secrets set "AppSettings:TokenKey" "your-super-secret-jwt-key-at-least-32-characters-long"
dotnet user-secrets set "AppSettings:PasswordKey" "your-password-encryption-key-at-least-32-chars"
```

### 3. Database Setup

1. Create database: `NotesAppDb`
2. Run SQL scripts from `src/SimpleNotesApp.Infrastructure/Stored Procedures/`

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
| `POST` | `/api/auth/register` | Register new user | ❌ |
| `POST` | `/api/auth/login` | User login | ❌ |
| `GET` | `/api/auth/refresh-token` | Refresh JWT token | ❌ |
| `PATCH` | `/api/auth/change-password` | Change password | ✅ |

### Notes
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| `GET` | `/api/notes` | Get all user notes | ✅ |
| `GET` | `/api/notes/{id}` | Get specific note | ✅ |
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

## 🏗️ Development

```bash
# Build the solution
dotnet build

# Run with hot reload
dotnet watch run --project src/SimpleNotesApp.API

# Check all is working
curl https://localhost:7108/swagger/index.html
```
