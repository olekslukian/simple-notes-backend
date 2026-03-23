# SimpleNotesApp Backend

A REST API for a note-taking application built with clean architecture. Supports JWT authentication, refresh tokens, and passwordless OTP login via email.

**Client App (in proggress)**: [simple-notes-client](https://github.com/olekslukian/simple-notes-client)

## Features

- CRUD operations for notes
- JWT authentication with refresh tokens
- Passwordless login via OTP email
- Traditional email/password authentication
- Unit tests for business logic and API controllers

## Tech Stack

- **.NET 8.0** — Web API framework
- **Dapper 2.1.35** — Lightweight ORM for SQL Server stored procedures
- **Microsoft SQL Server** — Database with stored procedures
- **JWT Bearer** — Token-based authentication
- **Mailgun** — Email service for OTP delivery
- **Swagger/OpenAPI** — API documentation
- **xUnit + Moq + FluentAssertions** — Testing

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

Dependency flow: `API → Core ← Infrastructure`

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

1. Create a database named `NotesAppDb`
2. Execute SQL scripts from `Stored Procedures/` in order:
   - `spRegistration_Upsert.sql` — User registration
   - `spCheck_User.sql` — Email verification
   - `spUser_Auth_Confirmation.sql` — Password authentication
   - `spUser_getById.sql`, `spUserId_get.sql` — User lookup
   - `spUser_getByRefToken.sql`, `spUser_RefreshToken_Update.sql` — Token management
   - `spNote_*.sql` — Notes CRUD operations

### 4. Run the Application

```bash
# From the solution root
dotnet run --project src/SimpleNotesApp.API
```

- HTTPS: `https://localhost:7108`
- HTTP: `http://localhost:5108`
- Swagger UI: `https://localhost:7108/swagger`

## API Endpoints

### Authentication

| Method | Endpoint | Description | Auth required |
|--------|----------|-------------|---------------|
| `POST` | `/api/auth/register` | Register with email and password | No |
| `POST` | `/api/auth/login` | Login with email and password | No |
| `POST` | `/api/auth/send-otp-login` | Request OTP for passwordless login | No |
| `POST` | `/api/auth/verify-email-login` | Verify email and complete OTP login | No |
| `POST` | `/api/auth/send-otp-password` | Request OTP to set a password | Yes |
| `POST` | `/api/auth/set-password` | Set password using OTP | Yes |
| `POST` | `/api/auth/change-password` | Change password (requires current password) | Yes |
| `POST` | `/api/auth/refresh-token` | Refresh JWT using refresh token | No |

### Notes

| Method | Endpoint | Description | Auth required |
|--------|----------|-------------|---------------|
| `GET` | `/api/notes` | Get all notes for the current user | Yes |
| `GET` | `/api/notes/{id}` | Get a specific note by ID | Yes |
| `POST` | `/api/notes` | Create a new note | Yes |
| `PATCH` | `/api/notes/{id}` | Update a note | Yes |
| `DELETE` | `/api/notes/{id}` | Delete a note | Yes |

## Testing the API

A Postman collection is available in the [Releases](https://github.com/olekslukian/simple-notes-backend/releases) section. Import it into Postman to quickly test all endpoints with pre-configured requests.

The live API is hosted at `https://api-simplenotes.olekslukian.xyz`.

### Register a new user

```bash
curl -X POST https://api-simplenotes.olekslukian.xyz/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test123!",
    "passwordConfirmation": "Test123!"
  }'
```

### Login

```bash
curl -X POST https://api-simplenotes.olekslukian.xyz/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test123!"
  }'
```

The response includes a `token` and a `refreshToken`. Use the `token` as a Bearer token for authenticated requests.

### Create a note

```bash
curl -X POST https://api-simplenotes.olekslukian.xyz/api/notes \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <your_token>" \
  -d '{
    "title": "My first note",
    "content": "This is the note content."
  }'
```

### Get all notes

```bash
curl https://api-simplenotes.olekslukian.xyz/api/notes \
  -H "Authorization: Bearer <your_token>"
```

### Update a note

```bash
curl -X PATCH https://api-simplenotes.olekslukian.xyz/api/notes/<note_id> \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <your_token>" \
  -d '{
    "title": "Updated title",
    "content": "Updated content."
  }'
```

### Delete a note

```bash
curl -X DELETE https://api-simplenotes.olekslukian.xyz/api/notes/<note_id> \
  -H "Authorization: Bearer <your_token>"
```

### Refresh token

```bash
curl -X POST https://api-simplenotes.olekslukian.xyz/api/auth/refresh-token \
  -H "Content-Type: application/json" \
  -d '{
    "token": "<your_token>",
    "refreshToken": "<your_refresh_token>"
  }'
```

## Running Tests

```bash
# Run all tests
dotnet test

# Run a specific test project
dotnet test tests/SimpleNotesApp.Core.Tests
dotnet test tests/SimpleNotesApp.API.Tests

# Run with detailed output
dotnet test --verbosity normal
```

Test coverage:
- Core Tests (44 tests) — AuthService, NotesService, AuthHelper
- API Tests (24 tests) — AuthController, NotesController

## Development

```bash
# Build the solution
dotnet build

# Run with hot reload
dotnet watch run --project src/SimpleNotesApp.API
```

