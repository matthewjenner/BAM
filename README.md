# Stargate - Astronaut Career Tracking System (ACTS)

## Project Overview

This repository contains a complete full-stack application for managing astronaut career tracking data. The system consists of a .NET 8 Web API backend and an Angular 20 Single Page Application frontend.

## Architecture

- **Backend**: .NET 8 Web API with Entity Framework Core and SQLite
- **Frontend**: Angular 20 SPA with modern TypeScript
- **Database**: SQLite with Entity Framework migrations
- **Patterns**: CQRS with MediatR, Repository pattern, Service layer

## Quick Start

### Prerequisites

- .NET 8.0 SDK
- Node.js 18.x or higher
- Angular CLI 20.x

### Running the Application

1. **Start the API**:

   ```bash
   cd API
   dotnet restore
   dotnet run --launch-profile https
   ```

   API will be available at `https://localhost:7204`

2. **Start the UI** (in a new terminal):

   ```bash
   cd UI
   npm install
   ng serve
   ```

   UI will be available at `http://localhost:4200`

3. **Access the Application**:
   - Open `http://localhost:4200` in your browser
   - API documentation available at `https://localhost:7204/swagger`

## Project Structure

```
BAM/
├── API/                   # .NET 8 Web API
│   ├── Business/          # CQRS commands/queries
│   ├── Controllers/       # API endpoints
│   ├── Tests/             # Unit tests
│   └── README.md          # API documentation
├── UI/                    # Angular 20 SPA
│   ├── src/app/           # Angular application
│   ├── src/app/features/  # Feature modules
│   └── README.md          # UI documentation
├── original/              # Original codebase
├── CODE_REVIEW_SUMMARY.md # Technical analysis
└── README.md              # This file
```

## Features

### API Features

- RESTful API endpoints for astronaut management
- CQRS pattern with MediatR
- Entity Framework Core with SQLite
- Comprehensive input validation
- Database logging and audit trails
- 50 unit tests with >50% coverage

### UI Features

- Modern Angular 20 SPA
- Responsive design
- Astronaut management interface
- Duty assignment system
- Real-time form validation
- Professional user experience

## Documentation

- **[API Documentation](API/README.md)** - Backend setup and API details
- **[UI Documentation](UI/README.md)** - Frontend setup and features
- **[Code Review Summary](CODE_REVIEW_SUMMARY.md)** - Comprehensive technical analysis

## Development

### API Development

```bash
cd API
dotnet test
dotnet run --launch-profile https
```

### UI Development

```bash
cd UI
npm install
ng serve
ng build
```

## Testing

- **API Tests**: 50 unit tests covering all business logic
- **UI Tests**: Angular unit tests (configure as needed)
- **Integration**: Full-stack testing with real API calls

## Security Features

- SQL injection prevention through Entity Framework
- Input validation at multiple layers
- CORS configuration for cross-origin requests
- Error message sanitization
- Comprehensive audit logging

## Technology Stack

### Backend

- .NET 8
- Entity Framework Core
- MediatR (CQRS)
- SQLite
- xUnit, Moq, FluentAssertions

### Frontend

- Angular 20
- TypeScript
- SCSS
- Angular Router
- Angular Forms

## Contributing

1. Follow the coding standards in each project
2. Write tests for new features
3. Update documentation as needed
4. Ensure both API and UI work together

## Support

For technical details and implementation decisions, see the comprehensive [Code Review Summary](CODE_REVIEW_SUMMARY.md).

For setup and usage instructions, see the individual README files in the API and UI directories.
