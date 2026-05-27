# AGENTS.md — Beach.SA API

## Project Overview

ASP.NET Core 8 Web API (C# 12) with Entity Framework Core 9 + SQL Server.  
Manages clients (`Cliente`), packages (`Paquete`), and reservations (`Reservacion`) for a beach resort.  
External API integrations:
- `https://apis.gometa.org/cedulas/{cedula}` for Costa Rican ID validation
- BCCR API (`gee.bccr.fi.cr`) for official USD/CRC exchange rate (falls back to config default)
- QuestPDF (2026.5) for PDF invoice generation
- `System.Net.Mail.SmtpClient` for email delivery with PDF attachments

## Directory Structure

```
API/
  API.slnx                        # Solution file
  API/                            # Web API project
    API.csproj
    Program.cs                    # Entry point, DI registration
    appsettings.json              # Connection strings
    Controllers/                  # API endpoints
    Models/                       # EF Core entities
    DTO/                          # Data Transfer Objects
    Services/                     # HTTP clients, external API calls, PDF, Email
    Repository/                   # DbContext
    Migrations/                   # EF Core migrations
    Properties/                   # launchSettings.json
```

## Build / Run / Migrate Commands

```bash
# Build the solution
dotnet build API/API.slnx

# Build the project
dotnet build API/API/API.csproj

# Run with hot reload
dotnet watch run --project API/API/API.csproj

# Run normally
dotnet run --project API/API/API.csproj

# Add a new EF Core migration
dotnet ef migrations add MigrationName --project API/API/API.csproj

# Apply migrations to the database
dotnet ef database update --project API/API/API.csproj
```

## Testing

No test project exists yet. When adding tests:
- Create a xUnit test project at `API/API.Tests/API.Tests.csproj`
- Name test classes `{Entity}ControllerTests` (e.g., `ClienteControllerTests`)
- Run all tests: `dotnet test API/API.slnx`
- Run a single test: `dotnet test API/API.slnx --filter "FullyQualifiedName~ClienteControllerTests.Create_CreatesCliente"`
- Name test methods as `{Method}_{Scenario}_ExpectedBehavior` (e.g., `Create_DuplicateCedula_ReturnsBadRequest`)

## Code Style Guidelines

### Namespaces & Usings
- `using` statements inside the namespace block (not outside)
- Group: System namespaces first, then third-party, then project-internal
- Remove unused usings
```csharp
namespace API.Controllers
{
    using API.Models;
    using API.Repository;
    using Microsoft.AspNetCore.Mvc;
}
```

### Naming Conventions
| Element | Convention | Example |
|---------|-----------|---------|
| Classes / Entities | PascalCase | `Cliente`, `Paquete`, `ClienteController` |
| Properties / Fields (public) | PascalCase | `Cedula`, `NombreCompleto`, `PrecioPorNoche` |
| Private fields | `_camelCase` | `_dbContextBeach`, `_apiServices` (preferred over `this.`) |
| Method parameters | camelCase | `cedula`, `nuevosDatos` |
| Local variables | camelCase | `clienteBuscado`, `paqueteExistente` |
| Methods | PascalCase | `List()`, `Create()`, `Edit()`, `Delete()`, `Search()` |
| DTO classes | PascalCase + `DTO` suffix | `ClienteGometaDTO` |
| Interfaces (if added) | `I` prefix | `IClienteRepository` |

### Formatting
- 4-space indentation, no tabs
- Braces on same line (K&R/Allman style)
- Line breaks between logical sections
- No `<PackageReference>` version attributes in `.csproj` comments

### Controllers
- Decorate with `[ApiController]` and `[Route("api/[controller]")]`
- Inherit from `ControllerBase`
- Route per action: combine `[HttpGet]` / `[HttpPost]` / `[HttpPut]` / `[HttpDelete]` with `[Route("ActionName")]`
- Standard CRUD actions: `List`, `Create`, `Update` (named `Edit`), `Delete`, `Search`
- Return `IActionResult` consistently (`Ok()`, `BadRequest()`, `NotFound()`)
- Inject dependencies via constructor and store as `_camelCase` fields
- Use `readonly` for injected fields where possible

### Models / Entities
- Data annotations for validation: `[Key]`, `[Required]`, `[StringLength]`, `[Range]`, `[EmailAddress]`, `[Phone]`
- `[JsonIgnore]` on navigation properties to avoid serialization cycles
- `[ForeignKey("PropertyName")]` on navigation properties
- Nullable reference types enabled (`string?` for optional fields)

### Error Handling
- Controllers: return HTTP-status-appropriate `IActionResult` (never raw `string` or entity types)
- Guard clauses: check for null/duplicates first, return `BadRequest` or `NotFound`
- Services layer: use try/catch only for boundary-crossing operations (HTTP calls, DB); let exceptions propagate in controllers
- Prefer specific error messages in Spanish matching existing convention

### Dependency Injection
- Register in `Program.cs`:
  - Transient for stateless services: `builder.Services.AddTransient<IService, Service>()`
  - Scoped for DbContext: via `AddDbContext<>`
  - Typed HttpClient: `builder.Services.AddHttpClient<IService, Service>(c => { c.Timeout = ...; })`
- Controller constructor injection for all dependencies
- For fire-and-forget background tasks (PDF/email after reservation), use `Task.Run()` inside the controller action with explicit try-catch and ILogger error logging

### Migrations
- One migration per logical change
- Run `dotnet ef migrations add` from the project directory
- Never edit migration files manually after generation

### DTOs
- Suffix class name with `DTO`
- Keep flat (no nested objects)
- Use for external API responses and request payloads distinct from entities

### General
- Prefer explicit types over `var` for non-obvious types
- Use `var` when type is clear from right-hand side (e.g., `var list = new List<int>()`)
- Comments in Spanish, matching existing convention
- Do not commit secrets or connection strings; use `appsettings.Development.json` / user secrets
- Keep controllers thin; extract business logic into Services when it grows beyond simple CRUD
