# dotnet-clean-architecture-api

REST API built with .NET following Clean Architecture principles, designed as a reference for enterprise grade applications.

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4)
![EF Core](https://img.shields.io/badge/EF%20Core-9.0-512BD4)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927)
![Docker](https://img.shields.io/badge/Docker-Enabled-2496ED)
![License](https://img.shields.io/badge/license-MIT-green)

## Description

A product and category management API that demonstrates how to organize a .NET codebase using Clean Architecture. The
solution separates business rules from frameworks and infrastructure, keeps dependencies pointing inwards, and covers
the concerns a real production service needs: validation, structured logging, error handling, health checks, tests and
containerization.

## Objective

Show, in a small and readable codebase, how to apply:

- Clean Architecture layering and dependency direction
- Repository pattern with Unit of Work
- Result pattern instead of exceptions for expected failures
- RFC 7807 ProblemDetails responses
- Request validation decoupled from controllers
- Unit and integration testing strategies

## Technologies

| Area | Stack |
| --- | --- |
| Runtime | .NET 9, ASP.NET Core Web API |
| Persistence | Entity Framework Core 9, SQL Server 2022 |
| Validation | FluentValidation |
| Documentation | Swagger / OpenAPI (Swashbuckle) |
| Logging | Serilog |
| Testing | xUnit, NSubstitute, FluentAssertions |
| Infrastructure | Docker, Docker Compose, GitHub Actions |

## Features

- Create, update, read and soft delete products
- Product listing with pagination, sorting and filters by name, category and status
- Category management with soft delete
- Referential validation between products and categories
- FluentValidation rules applied automatically to incoming requests
- Global exception handling returning ProblemDetails
- Structured logging with Serilog, including request logging
- Liveness and readiness health check endpoints
- Database migrations and optional seed applied on startup

## Architecture

```
+-----------------------------------------------------+
|                       WebApi                        |
|   Controllers, middleware, composition root         |
+--------------------------+--------------------------+
                           |
+--------------------------v--------------------------+
|                   Infrastructure                    |
|   EF Core, DbContext, repositories, migrations      |
+--------------------------+--------------------------+
                           |
+--------------------------v--------------------------+
|                    Application                      |
|   Services, DTOs, validators, Result pattern        |
+--------------------------+--------------------------+
                           |
+--------------------------v--------------------------+
|                       Domain                        |
|   Entities, business rules, repository contracts    |
+-----------------------------------------------------+
```

Dependencies always point inwards. `Domain` has no dependency on any other project and no dependency on Entity
Framework. Repository contracts live in `Domain` and are implemented in `Infrastructure`, so the business rules never
know which database is being used.

## Project structure

```
src/
  Domain/                     Entities, domain exceptions, repository contracts
  Application/                Use case services, DTOs, validators, Result pattern
  Infrastructure/             EF Core DbContext, configurations, repositories, migrations
  WebApi/                     Controllers, middleware, dependency injection, Swagger
tests/
  UnitTests/                  Domain rules and application services
  IntegrationTests/           Endpoint tests using WebApplicationFactory
```

## How to run

### With Docker

```bash
git clone https://github.com/lucas-goncalves-cav/dotnet-clean-architecture-api.git
cd dotnet-clean-architecture-api
cp .env.example .env
docker compose up -d
```

The API becomes available at `http://localhost:8080` and Swagger UI at `http://localhost:8080/swagger`.

### Locally

```bash
dotnet restore
dotnet run --project src/WebApi
```

Update the connection string in `src/WebApi/appsettings.json` or provide it through the
`ConnectionStrings__DefaultConnection` environment variable before starting.

### Tests

```bash
dotnet test
```

## Configuration

Copy `.env.example` to `.env` and adjust the values.

| Variable | Description | Default |
| --- | --- | --- |
| `MSSQL_SA_PASSWORD` | SQL Server sa password used by Docker Compose | `your_password_here` |
| `SQLSERVER_PORT` | Host port mapped to SQL Server | `1433` |
| `DATABASE_NAME` | Database created by the API | `CleanArchitectureDb` |
| `API_PORT` | Host port mapped to the API | `8080` |
| `ASPNETCORE_ENVIRONMENT` | ASP.NET Core environment | `Development` |

Application settings that can be overridden:

| Setting | Description |
| --- | --- |
| `ConnectionStrings:DefaultConnection` | SQL Server connection string |
| `Database:ApplyMigrationsOnStartup` | Applies pending EF Core migrations when the API starts |
| `Database:SeedOnStartup` | Inserts demo categories and products when the database is empty |

No real credentials are stored in this repository.

## Main endpoints

### Products

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/api/products` | Lists products with pagination, sorting and filters |
| `GET` | `/api/products/{id}` | Gets a product by identifier |
| `POST` | `/api/products` | Creates a product |
| `PUT` | `/api/products/{id}` | Updates a product |
| `DELETE` | `/api/products/{id}` | Soft deletes a product |

Supported query string parameters on the listing endpoint:

```
name=mouse
categoryId=3fa85f64-5717-4562-b3fc-2c963f66afa6
active=true
sortBy=price          (name | price | createdAt)
sortDescending=true
page=1
pageSize=20
```

### Categories

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/api/categories` | Lists categories, optionally filtered by status |
| `GET` | `/api/categories/{id}` | Gets a category by identifier |
| `POST` | `/api/categories` | Creates a category |
| `PUT` | `/api/categories/{id}` | Updates a category |
| `DELETE` | `/api/categories/{id}` | Soft deletes a category |

### Health

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/health/live` | Liveness probe |
| `GET` | `/health/ready` | Readiness probe, includes SQL Server connectivity |

### Request example

```bash
curl -X POST http://localhost:8080/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Mechanical Keyboard",
    "description": "Compact mechanical keyboard",
    "price": 459.00,
    "categoryId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
  }'
```

### Response example

```json
{
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "Mechanical Keyboard",
      "description": "Compact mechanical keyboard",
      "price": 459.00,
      "categoryId": "9d2f1c44-0f2c-4d1e-9c1a-5f8a3b2d7e10",
      "categoryName": "Electronics",
      "active": true,
      "createdAt": "2026-01-15T12:00:00Z",
      "updatedAt": null
    }
  ],
  "totalItems": 1,
  "page": 1,
  "pageSize": 20,
  "totalPages": 1,
  "hasPrevious": false,
  "hasNext": false
}
```

Errors follow the ProblemDetails format:

```json
{
  "title": "Resource not found",
  "status": 404,
  "detail": "Product 3fa85f64-5717-4562-b3fc-2c963f66afa6 was not found.",
  "instance": "/api/products/3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

## Design decisions

- **Soft delete over physical delete.** Products and categories carry an `Active` flag so history is preserved.
- **Result pattern.** Expected failures such as not found travel as values, not exceptions. Exceptions are reserved
  for invariant violations and unexpected errors.
- **Rich entities.** Setters are private and state changes go through methods that enforce the rules.
- **Repository contracts in Domain.** Keeps Entity Framework out of the business layer.
- **Sorting whitelist.** The listing endpoint only accepts known sort fields, avoiding injection through query strings.

## Roadmap

- [ ] JWT authentication and role based authorization
- [ ] Redis caching for product listings
- [ ] Outbox pattern for domain events
- [ ] OpenTelemetry traces and metrics
- [ ] Rate limiting on public endpoints

## License

Distributed under the MIT License. See [LICENSE](LICENSE) for details.
