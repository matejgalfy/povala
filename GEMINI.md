# Povala Project - Agent Guidelines

## General Coding Standards
- **Professionalism**: Write clean, maintainable, production-ready code. Adhere strictly to SOLID principles and common C# conventions.
- **Decomposition**: Break down large methods into smaller, well-named, and focused helper functions. Keep classes focused on a single responsibility.
- **No Unnecessary Comments**: Avoid writing obvious or redundant comments (e.g., `// Returns the user list`). Only use comments to explain *why* complex or non-obvious technical decisions were made. 
- **No Prompt Comments**: NEVER leave conversational or prompt-related comments in the source code (e.g., `// Here is the new function as requested` or `// Updated logic`). Just write the raw code.

## Architecture Context
This solution is divided into three main projects:
1. **`Povala.Api`**: An ASP.NET Core Web API acting as the backend. It uses Entity Framework Core (`ApplicationDbContext`) and a SQLite database for persistence.
2. **`povala-app`**: A cross-platform .NET MAUI application serving as the client frontend. It strictly follows the MVVM (Model-View-ViewModel) pattern.
3. **`Povala.Shared`**: A class library containing shared code like DTOs, Enums, and Constants. When updating models or API contracts, make sure to update the DTOs here so both the frontend and backend remain in sync.

## Specific Framework Guidelines
- **C# Practices**: Use modern C# features (like file-scoped namespaces, pattern matching, and record types for DTOs where appropriate). Always use `async`/`await` for I/O operations.
- **.NET MAUI (povala-app)**: Do not place business logic in the code-behind files (`.xaml.cs`). Bind UI elements in XAML and place all presentation logic inside the respective `ViewModels`.
- **ASP.NET Core (Povala.Api)**: Keep API controllers thin. Route complex business logic to dedicated service classes. 
- **Dependency Injection**: Fully leverage standard .NET dependency injection to pass services, view-models, and DbContexts around.

## Documentation Maintenance
- **`README.md`**: Continuously update the README when new features are added, dependencies change, or setup instructions are modified to ensure onboarding remains seamless.
- **`GEMINI.md` (This File)**: Treat this file as a living document. Whenever new project-wide conventions are established, new architectural decisions are made, or new patterns emerge in the codebase, update this file to ensure future agents are aligned.