# String Analysis API

A small, opinionated .NET 9 Web API that demonstrates a clean layered architecture for analyzing and persisting string data. The project separates concerns into Controller ? Service ? Repository layers and uses DI with interface-driven repositories to make components testable and replaceable (in-memory implementation provided).

## Key characteristics
- Target framework: .NET 9
- Layered design: Presentation (Controllers) ? Business (Services) ? Data (Repositories)
- Interface-first design for easy swapping/mocking (`IStringRepository`)
- In-memory repository included (`InMemoryStringRepository`) for local/dev runs
- Docker-ready (`Dockerfile`)

## Project layout
- `Program.cs` — bootstraps the app and configures DI and routing
- `Controllers/StringsController.cs` — HTTP endpoints (presentation layer)
- `Service/StringAnalysisService.cs` — business logic for analyzing strings
- `Repositories/IStringRepository.cs` — repository contract
- `Repositories/InMemoryStringRepository.cs` — in-memory repository implementation
- `Entity/StringRequest.cs` — request DTO
- `Entity/AnalyzedString.cs` — domain/response entity

This maps to a simple, testable layered architecture:
- Controller: accepts requests, validates minimal input, returns HTTP responses
- Service: contains pure business logic (string analysis)
- Repository: persistence abstraction (interface + implementation)

## Design intent & benefits
- Single Responsibility: each layer has a focused purpose
- Testability: swap `IStringRepository` for a mock in unit tests
- Maintainability: business rules live in `Service` and are independent of transport and storage
- Replaceability: swap `InMemoryStringRepository` for a DB-backed repository without changing controllers or services

## Typical endpoints (examples)
Note: adapt to actual route names in `StringsController`.

- POST /api/strings/analyze
  - Request body: `{"text":"some string"}`
  - Response: `{"id":"...", "text":"some string","length":..., "wordCount":..., ...}`

- GET /api/strings/{id}
  - Returns the analyzed string entity for the given id

## Build & run

Command-line:
- Restore and build:
  - `dotnet restore`
  - `dotnet build`
- Run locally:
  - `dotnet run` (from project folder)

Visual Studio:
- Open solution, then use __F5__ to start debugging or __Debug > Start Debugging__ to run with debugger attached.
- Use __Ctrl+F5__ to run without debugging.

Docker:
- Build: `docker build -t string-analysis-api .`
- Run: `docker run -p 5000:80 string-analysis-api`

## Configuration & extension points
- Replace `InMemoryStringRepository` with a database-backed implementation by creating a new class that implements `IStringRepository` and registering it in `Program.cs`.
- Add more analysis features in `StringAnalysisService` (e.g., sentiment, tokenization) without changing controllers.

## Testing
- Unit test services and controllers by mocking `IStringRepository`.
- Keep business rules in `StringAnalysisService` small and pure to simplify unit testing.

## Contributing
- Follow the existing layering (Controller ? Service ? Repository).
- Keep changes small and focused; add unit tests for new business rules.
- Open PRs with clear descriptions of behavior changes.

## License
- Add a LICENSE file to declare project licensing.

If you want, I can:
- generate a `README.md` file and commit it,
- produce a sample `docker-compose.yml`,
- or scaffold a DB-backed `IStringRepository` implementation (e.g., EF Core).