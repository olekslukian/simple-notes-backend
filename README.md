# SimpleNotesApp Backend

A REST API for a note-taking application built with .NET 8, using JWT authentication and SQL Server.

## Client
Client is implemented in [Android app](https://github.com/olekslukian/simple-notes-android)

## Tech Stack

- .NET 8.0 Web API
- Dapper ORM
- Microsoft SQL Server
- JWT Authentication
- Swagger UI

## Quick Start

### Prerequisites
- .NET 8.0 SDK
- SQL Server

### Setup

1. **Clone the repository**

2. **Update connection string**
Edit `src/SimpleNotesApp.API/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=NotesAppDb;Trusted_Connection=true;TrustServerCertificate=true;"
  },
  "AppSettings": {
    "TokenKey": "your-super-secret-jwt-key-must-be-at-least-32-characters-long",
    "PasswordKey": "your-super-secret-password-key-must-be-at-least-32-characters-long"
  }
}
```

3. **Create database and run SQL scripts**
- Create `NotesAppDb` database
- Run scripts from `src/SimpleNotesApp.Infrastructure/Stored Procedures/`

4. **Run the application**
```bash
dotnet run --project src/SimpleNotesApp.API
```

Visit: `https://localhost:7108/swagger`

## API Endpoints

### Auth
- `POST /api/auth/register` - Register user
- `POST /api/auth/login` - Login user  
- `GET /api/auth/refresh-token` - Refresh JWT token
- `PATCH /api/auth/change-password` - Change password (requires auth)

### Notes
- `GET /api/notes` - Get user notes (requires auth)
- `POST /api/notes` - Create note (requires auth)
- `PUT /api/notes/{id}` - Update note (requires auth)
- `DELETE /api/notes/{id}` - Delete note (requires auth)

## Project Structure

```
src/
├── SimpleNotesApp.API/           # Controllers, Program.cs
├── SimpleNotesApp.Core/          # Business logic, DTOs, Interfaces
└── SimpleNotesApp.Infrastructure/ # Data access, Models, Stored Procedures
```

## Coming Soon

- Email verification
- Password reset
- Docker deployment
- VPS deployment guide

## Development

```bash
# Build
dotnet build

# Run
dotnet run --project src/SimpleNotesApp.API

# Test API
curl -X POST https://localhost:7108/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"Test123!","passwordConfirmation":"Test123!"}'
```
