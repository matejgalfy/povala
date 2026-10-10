# Povala - Bouldering App

Povala is a simplified application designed for managing and navigating a single specific bouldering gym (similar to TopLogger). It consists of a backend (ASP.NET Core API) and a client application (.NET MAUI).

### User Roles
The application uses a system of two roles:
1. **Owner:** 
   - Has access to the map management interface.
   - Can add a new boulder by tapping on the map, specifying its color and difficulty (coordinates are saved automatically).
   - Can soft delete existing boulders by tapping on them.
2. **User (Climber):**
   - Has access to the standard user view (`UserShell`).
   - Can see the visual map of the gym with all current boulders.
   - Upon clicking a boulder, the user can mark it as an **Ascent** or as a **Project**.
   - Has a profile page with statistics: total boulders climbed, number of active projects and highest grade climbed.

### Project Structure
- `Povala.Api/` - ASP.NET Core REST API. Handles data persistence (SQLite via Entity Framework Core), user authentication (Identity), and the logic for boulders, ascents, and projects.
- `povala-app/` - .NET MAUI frontend application for mobile devices and desktop, separated into modules for the Owner and regular User.
- `Povala.Shared/` - Shared library containing common constants (e.g., roles, difficulty and color enums) and DTOs (Data Transfer Objects) shared between the API and the MAUI app.

## Local Setup

### Backend (Povala.Api)
1. Navigate to the `Povala.Api` directory.
2. Verify and apply database migrations using the command: `dotnet ef database update`.
3. Run the API: `dotnet run`. The API will start and provide access to the endpoints and the database (`povala.db`).

### Frontend (povala-app)
1. Open the `.sln` solution in Visual Studio.
2. Set the `povala-app` project as the "Startup project".
3. Compile and run the application on your desired emulator (Android, iOS) or as a local Windows application (Windows Machine).

