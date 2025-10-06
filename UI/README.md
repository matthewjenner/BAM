# Stargate UI

## Overview

The Stargate UI is an Angular 20 Single Page Application (SPA) that provides a modern web interface for the Stargate API. It allows users to manage astronaut personnel records, view career details, and assign duties.

## Prerequisites

- Node.js 18.x or higher
- Angular CLI 20.x
- .NET 8 API running (see API README for setup)

## Getting Started

### 1. Install Dependencies

```bash
cd UI
npm install
```

### 2. Start the Development Server

```bash
ng serve
```

The application will be available at `http://localhost:4200`

### 3. Build for Production

```bash
ng build
```

The build artifacts will be stored in the `dist/` directory.

## Features

### Astronaut Management

- View all astronauts with their current details
- Search and filter astronaut records
- View detailed astronaut profiles with career history
- Edit astronaut information (rank, duty title, career dates)

### Duty Assignment

- Create new duty assignments for astronauts
- Select from predefined rank options
- Set duty start dates and titles
- View historical duty assignments

### User Interface

- Modern, responsive design
- Clean, professional appearance
- Intuitive navigation
- Real-time form validation
- Loading states and error handling

## Project Structure

```
UI/
├── src/
│   ├── app/
│   │   ├── core/              # Core services and constants
│   │   │   ├── constants/     # Application constants
│   │   │   ├── models/        # TypeScript interfaces
│   │   │   └── services/      # API services
│   │   ├── features/          # Feature modules
│   │   │   ├── home/          # Home page
│   │   │   ├── astronauts/    # Astronaut management
│   │   │   ├── duties/        # Duty assignment
│   │   │   └── astronaut-detail/ # Astronaut details
│   │   ├── app.component.*    # Root component
│   │   ├── app.config.ts      # App configuration
│   │   └── app.routes.ts      # Routing configuration
│   ├── index.html             # Main HTML file
│   └── main.ts                # Application entry point
├── angular.json               # Angular CLI configuration
├── package.json               # Dependencies and scripts
└── tsconfig.json              # TypeScript configuration
```

## Development

### Available Scripts

- `ng serve` - Start development server
- `ng build` - Build for production
- `ng test` - Run unit tests
- `ng lint` - Run linting
- `ng generate component <name>` - Generate new component
- `ng generate service <name>` - Generate new service

### Code Style

The project follows Angular best practices:

- Standalone components
- Reactive forms
- TypeScript strict mode
- SCSS for styling
- Consistent naming conventions

### API Integration

The UI communicates with the Stargate API through:

- HTTP services for data operations
- Reactive forms for user input
- Error handling and loading states
- CORS configuration for development

## Configuration

### API Endpoint

The API endpoint is configured in the core services. Update the base URL in `src/app/core/services/api.service.ts` if the API is running on a different port.

### Styling

The application uses SCSS for styling with:

- CSS classes instead of inline styles
- Responsive design principles
- Clean, professional appearance
- Consistent color scheme

## Features Overview

### Home Page

- Welcome message and system overview
- Navigation to main features
- Quick access to astronaut and duty management

### Astronauts Page

- Grid view of all astronauts
- Search and filter capabilities
- Add new astronauts
- View detailed astronaut profiles

### Astronaut Detail Page

- Complete astronaut information
- Career history and duty assignments
- Edit astronaut details
- Responsive design for all screen sizes

### Duties Page

- Create new duty assignments
- Select astronaut and rank
- Set duty details and dates
- Form validation and error handling

## Dependencies

### Core Dependencies

- **Angular 20** - Framework
- **Angular Router** - Navigation
- **Angular Forms** - Form handling
- **Angular HTTP Client** - API communication

### Development Dependencies

- **Angular CLI** - Development tools
- **TypeScript** - Language
- **SCSS** - Styling
- **Karma/Jasmine** - Testing

## Browser Support

The application supports modern browsers:

- Chrome 90+
- Firefox 88+
- Safari 14+
- Edge 90+

## Troubleshooting

### Common Issues

1. **API Connection Issues**

   - Ensure the API is running on the correct port
   - Check CORS configuration
   - Verify API endpoint URLs

2. **Build Issues**

   - Clear node_modules and reinstall: `rm -rf node_modules && npm install`
   - Update Angular CLI: `npm install -g @angular/cli@latest`

3. **Styling Issues**
   - Check SCSS compilation
   - Verify CSS class names
   - Ensure proper imports
