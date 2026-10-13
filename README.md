# Hotel Reservation API

A hotel reservation management system built with ASP.NET Core Web API and SQL Server. This project provides APIs for managing hotels, rooms, room types, users, and reservations.

## Technologies Used

- C#
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- JWT Authentication and Authorization
- BCrypt password hashing
- Swagger / OpenAPI
- Git and GitHub

## Features

### Authentication and Authorization
- User registration and login.
- JWT-based authentication.
- Role-based access control for Admin and Customer.
- Protected user profile endpoint.
- Forgot-password and reset-password functionality.

### Hotel Management
- Create, read, update, and delete hotels.
- Search hotels and filter by rating.
- Sort and paginate hotel results.

### Room Management
- Manage rooms and room types.
- Associate rooms with hotels and room types.
- Track room information and nightly prices.

### Reservation Management
- Create reservations.
- Check room availability for selected dates.
- Calculate the total reservation price.
- View personal reservations.
- Cancel reservations.
- Allow administrators to view reservations and update their statuses.
- Prevent overlapping reservations for the same room through availability checks.

## Project Structure

- `Controllers/` — API endpoints and request handling.
- `DTo/` — Data Transfer Objects for API requests and responses.
- `model/` — Entity models.
- `Services/` — Supporting services, including password-reset token generation.
- `Migrations/` — Entity Framework Core database migrations.
- `AppDbContext.cs` — Database context and entity configuration.
- `Program.cs` — Application configuration, dependency injection, authentication, and middleware.

## Getting Started

### Prerequisites

- Visual Studio 2022 or a compatible .NET development environment.
- The .NET SDK matching the project's target framework.
- SQL Server or SQL Server Express.
- Git.

### 1. Clone the Repository

```bash
git clone https://github.com/mohamedtarek80/HotelReservation.git
cd HotelReservation
```

Open the solution file in Visual Studio.

### 2. Configure User Secrets

Configure the following settings through ASP.NET Core User Secrets or environment variables:

- `ConnectionStrings:DefaultConnection`
- `Jwt:Key`
- `AdminSetup:Password`

Keep sensitive values out of source control.

The application also uses JWT settings such as `Jwt:Issuer`, `Jwt:Audience`, and `Jwt:DurationInMinutes`. Configure these consistently with the token-generation and validation code.

### 3. Configure the Database

Set `ConnectionStrings:DefaultConnection` to your SQL Server connection string.

Apply the existing Entity Framework Core migrations from the project directory:

```bash
dotnet ef database update
```

If the EF Core command-line tool is not installed, install it according to the official EF Core documentation.

### 4. Run the Application

Run the project from Visual Studio or use:

```bash
dotnet run
```

When running in Development, Swagger UI is enabled for exploring and testing the API endpoints.

## API Overview

| Feature | Endpoint | Access |
|---|---|---|
| Register | `POST /api/User/Register` | Anonymous |
| Login | `POST /api/User/login` | Anonymous |
| User profile | `GET /api/User/profile` | Authenticated |
| List users | `GET /api/User` | Admin |
| Admin test | `GET /api/User/admin-test` | Admin |
| List hotels | `GET /api/Hotel` | Depends on controller configuration |
| Manage rooms | `/api/Room` | Depends on controller configuration |
| Manage room types | `/api/RoomType` | Depends on controller configuration |
| Create reservation | `POST /api/Reservation` | Customer |
| My reservations | `GET /api/Reservation/my-reservations` | Customer |
| Check availability | `POST /api/Reservation/Check_Availability` | Anonymous |
| List all reservations | `GET /api/Reservation/all` | Admin |

Check the controllers for the complete routes and authorization requirements. Endpoint names and HTTP methods should match the actual code in the repository.

## Authentication

1. Register a user or use a configured administrator account.
2. Log in through `POST /api/User/login`.
3. Copy the JWT returned by the API.
4. Use Swagger's **Authorize** feature to test protected endpoints.
5. Use an account with the appropriate role for role-restricted operations.

## Security Notes

- Passwords are hashed using BCrypt.
- JWT signing keys and database credentials must be stored outside source control.
- Password-reset tokens expire and are cleared after a successful reset.
- Returning password-reset tokens is intended only for local Development testing; production deployments should deliver reset instructions through a secure channel.
- Review authorization policies and reservation concurrency handling before deploying to production.

## Future Improvements

- Email delivery for password-reset links.
- Automated unit and integration tests.
- More robust handling of concurrent reservations.
- Centralized exception handling and structured logging.
- Deployment to a cloud hosting platform.
- CI/CD automation.

## Author

**Mohamed Tarek**

GitHub: https://github.com/mohamedtarek80