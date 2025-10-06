# Stargate API

## Overview

The Stargate API is a .NET 8 Web API that provides RESTful endpoints for managing astronaut career tracking data. It implements a CQRS pattern using MediatR and uses Entity Framework Core with SQLite for data persistence.

## Prerequisites

- .NET 8.0 SDK
- Visual Studio 2022, VS Code, or Rider
- SQLite (included with .NET)

## Getting Started

### 1. Clone and Navigate

```bash
git clone <repository-url>
cd API
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Run the Application

```bash
dotnet run --launch-profile https
```

The API will be available at:

- HTTP: `http://localhost:5204`
- HTTPS: `https://localhost:7204`
- Swagger UI: `https://localhost:7204/swagger`

### 4. Run Tests

```bash
dotnet test
```

## API Endpoints

### Person Management

- `GET /Person` - Get all people
- `GET /Person/{name}` - Get person by name
- `POST /Person` - Create new person
- `PUT /Person/{name}` - Update person details

### Astronaut Duty Management

- `GET /AstronautDuty/{name}` - Get astronaut duties by name
- `POST /AstronautDuty` - Create new astronaut duty

## Database

The application uses SQLite with Entity Framework Core migrations. The database is automatically created and seeded with sample data on first run.

### Database Schema

- **Person** - Basic person information
- **AstronautDetail** - Current astronaut details (rank, duty, career dates)
- **AstronautDuty** - Historical duty assignments
- **LogEntries** - Application operation logs

## Configuration

### Connection String

The default connection string is configured in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "StarbaseApiDatabase": "Data Source=starbase.db"
  }
}
```

### CORS

CORS is configured for development to allow the Angular frontend to communicate with the API. In production, configure specific origins for security.

## Development Features

- **Automatic Database Initialization** - Database and migrations applied on startup
- **Seed Data** - Sample data (John Doe, Jane Doe) loaded in development
- **Comprehensive Logging** - All operations logged to database
- **Input Validation** - All endpoints validate input data
- **Error Handling** - Consistent error responses across all endpoints

## Testing

The project includes 50 unit tests covering:

- Controller endpoints
- Command handlers
- Query handlers
- Validation preprocessors
- Error scenarios

Run tests with:

```bash
dotnet test
```

## Project Structure

```
API/
├── Business/
│   ├── Commands/          # CQRS Commands
│   ├── Queries/           # CQRS Queries
│   ├── Data/              # Entity Framework models
│   ├── Services/          # Business services
│   └── Migrations/        # Database migrations
├── Controllers/           # API Controllers
├── Tests/                 # Unit tests
└── Program.cs             # Application startup
```

## Dependencies

- **MediatR** - CQRS pattern implementation
- **Entity Framework Core** - ORM and data access
- **SQLite** - Database provider
- **xUnit** - Testing framework
- **Moq** - Mocking framework
- **FluentAssertions** - Test assertions
- **Dapper** - Left as a precaution even though it is not being used

## Troubleshooting

### Database Issues

If you encounter database-related errors:

1. Delete the `starbase.db` file
2. Restart the application
3. The database will be recreated automatically

### Port Conflicts

If the default ports are in use, configure different ports in `Properties/launchSettings.json`.
