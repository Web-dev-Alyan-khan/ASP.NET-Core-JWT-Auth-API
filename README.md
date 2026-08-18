# ASP.NET Core JWT Authentication API

A secure ASP.NET Core Web API built with .NET 10, Entity Framework Core, SQL Server, JWT Authentication, Role-Based Authorization, Password Hashing, and Swagger/OpenAPI.

## 🚀 Features

- User Registration
- User Login
- Secure Password Hashing
- JWT Authentication
- Role-Based Authorization
- User Role
- Admin Role
- Protected APIs
- Entity Framework Core
- SQL Server Database
- EF Core Migrations
- Swagger/OpenAPI API Documentation
- RESTful Web API

## 🛠️ Technologies

- .NET 10
- ASP.NET Core Web API
- C#
- Entity Framework Core
- SQL Server
- JWT
- Swagger / OpenAPI
- Visual Studio Code
- Git & GitHub

## 📁 Project Structure

```text
MyWebApi
│
├── Controllers
│   ├── AuthController.cs
│   ├── UsersController.cs
│   └── AdminController.cs
│
├── Data
│   └── AppDbContext.cs
│
├── DTOs
│   ├── Auth
│   │   ├── RegisterDto.cs
│   │   └── LoginDto.cs
│   └── User
│       └── UserDto.cs
│
├── Helpers
│   └── PasswordHasher.cs
│
├── Models
│   ├── User.cs
│   └── Role.cs
│
├── Services
│   ├── IAuthService.cs
│   └── AuthService.cs
│
├── Migrations
│
├── Program.cs
├── appsettings.json
└── MyWebApi.csproj
