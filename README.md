# Logko API 🔐

A secure authentication REST API built with ASP.NET Core, demonstrating real-world security practices including JWT authentication, refresh token rotation, role-based access control, and brute force protection.

---

## Tech Stack

- **Framework** → ASP.NET Core 10
- **Database** → SQL Server + Entity Framework Core
- **Authentication** → JWT Bearer tokens
- **Password Hashing** → BCrypt
- **API Docs** → Swagger UI
- **Caching** → Redis (coming soon)

---

## Features

- ✅ User registration with BCrypt password hashing
- ✅ JWT access tokens (15 min expiry, zero clock skew)
- ✅ Refresh token rotation (7 day expiry)
- ✅ Role-based access control (Admin, User, Manager)
- ✅ Account lockout after 5 failed login attempts (15 min)
- ✅ Rate limiting on login endpoint (5 requests per 15 min)
- ✅ Global error handling middleware (clean JSON errors)
- ✅ Swagger UI with JWT authorization support
- 🔜 Redis token blacklisting for logout
- 🔜 Docker + deployment to Azure

---

## Architecture

Clean layered architecture following separation of concerns:

```
Request → Middleware → Controller → Service → Repository → Database
```

```
Logko.API/
├── Controllers/     → handle HTTP requests, return responses
├── Services/        → business logic (auth, JWT generation)
├── Data/            → repository pattern, EF Core queries
├── Models/          → domain entities (User)
├── DTOs/            → request/response contracts
├── Middleware/      → global error handling
└── Migrations/      → EF Core database migrations
```

---

## Security Decisions

| Decision | Reason |
|---|---|
| BCrypt over MD5/SHA256 | BCrypt is intentionally slow — work factor makes brute force expensive |
| 15 min access token expiry | Limits damage window if token is stolen |
| Refresh token rotation | Old token invalidated on each use — detects theft |
| Zero ClockSkew | Strict token expiry, no grace period |
| Role embedded in JWT | No extra DB call needed on every request |
| Account lockout | Prevents targeted brute force attacks regardless of IP |
| Rate limiting | Stops automated bot attacks on login endpoint |
| DTOs over raw models | Prevents sensitive data (PasswordHash) from leaking to client |

---

## Prerequisites

- .NET 10 SDK
- SQL Server + SSMS
- Visual Studio Code or Visual Studio

---

## Packages

```
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package BCrypt.Net-Next
dotnet add package StackExchange.Redis
dotnet add package Swashbuckle.AspNetCore
```

---

## Getting Started

**1. Clone the repo**
```
git clone https://github.com/HuyNLy/LogKo.git
cd LogKo
```

**2. Update `appsettings.json`**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=Logko;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "JwtSettings": {
    "Secret": "your-secret-key-at-least-32-characters",
    "ExpiryMinutes": 15,
    "RefreshTokenExpiryDays": 7
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  }
}
```

**3. Run migrations**
```
dotnet ef database update
```

**4. Run the app**
```
dotnet run
```

**5. Open Swagger**
```
http://localhost:5018/swagger
```

---

## API Endpoints

| Method | Endpoint | Description | Auth |
|---|---|---|---|
| POST | `/api/auth/register` | Register new user | None |
| POST | `/api/auth/login` | Login, returns JWT | None |
| POST | `/api/auth/refresh` | Refresh access token | None |
| GET | `/api/auth/protected` | Test protected route | Any role |
| GET | `/api/auth/admin-only` | Admin only route | Admin |
| GET | `/api/auth/user-only` | User only route | User |

---

## How Authentication Works

```
1. Register → password hashed with BCrypt → saved to DB
2. Login → BCrypt verify → JWT generated with claims (id, username, role)
3. Access token expires (15 min) → client sends refresh token
4. Server validates refresh token → issues new access + refresh token
5. Old refresh token invalidated (rotation)
```

---

## What I'd Add Next

- **Redis** → token blacklisting on logout, distributed lockout tracking
- **Docker** → containerize app + SQL Server + Redis
- **CI/CD** → GitHub Actions pipeline (build, test, deploy)
- **Azure** → deploy to Azure App Service
- **Frontend** → React login page, user dashboard, admin page