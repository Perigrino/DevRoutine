# DevRoutine

DevRoutine is a RESTful API for tracking developer habits and routines. It lets you define routines—such as coding hours, debugging sessions, or learning new tech—with targets, frequency, and milestones, then query, filter, and analyze them from any tool you integrate with.

Built on .NET 10 with ASP.NET Core, EF Core, and PostgreSQL.

## Capabilities

- **Routine management** — create, read, update, patch, and delete routines with full validation (FluentValidation).
- **Routine modeling** — routines support a type (binary/measurable), frequency (daily/weekly/monthly), a target value with unit, milestones, status, archiving, and an optional end date.
- **Tagging** — tag routines and manage tags via a dedicated `tags` resource; upsert and remove tag associations per routine.
- **Sorting** — sort by any whitelisted field via `?sort=name desc` using dynamic LINQ (with a mapping whitelist to prevent injection).
- **Data shaping** — request only the fields you need with `?fields=id,name`.
- **Pagination** — paged responses with total counts, total pages, and previous/next-page links.
- **HATEOAS** — every routine response carries discoverable links (self, update, patch, delete, upsert-tags).
- **Problem Details** — RFC 7807 error responses via exception handlers for validation and server errors.
- **OpenAPI** — automatic API documentation exposed in development.
- **Observability** — OpenTelemetry traces, metrics, and logs exported over OTLP (works out of the box with the included Aspire dashboard and Seq).
- **Resilient data access** — EF Core over PostgreSQL with snake_case naming conventions and retry-on-failure.

## Tech Stack

- .NET 10 / ASP.NET Core Web API
- Entity Framework Core 10 + Npgsql (PostgreSQL)
- FluentValidation
- OpenTelemetry (OTLP exporter)
- Docker Compose (PostgreSQL, Seq, Aspire dashboard)

## Getting Started

Requirements: [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) and Docker.

```bash
# Start the backing services (PostgreSQL, Seq, Aspire dashboard)
docker compose up -d

# Run the API
dotnet run --project DevRoutine/DevRoutine.Api
```

In development the API listens on `http://localhost:5010` (HTTPS on `https://localhost:5011`), applies EF migrations automatically, and exposes its OpenAPI document.

## Endpoints

| Method | Route                          | Description                       |
| ------ | ------------------------------ | --------------------------------- |
| GET    | `/routines`                    | List routines (filter/sort/paginate) |
| GET    | `/routines/{id}`               | Get a single routine with its tags |
| POST   | `/routines`                    | Create a routine                  |
| PUT    | `/routines/{id}`               | Update a routine                  |
| PATCH  | `/routines/{id}`               | Partially update a routine        |
| DELETE | `/routines/{id}`               | Delete a routine                  |
| PUT    | `/routines/{routineId}/tags`   | Replace the routine's tags        |
| DELETE | `/routines/{routineId}/tags/{tagId}` | Remove a tag from a routine  |
| GET    | `/tags`                        | List tags                         |
| GET    | `/tags/{id}`                   | Get a single tag                  |
| POST   | `/tags`                        | Create a tag                      |
| PUT    | `/tags/{id}`                   | Update a tag                      |
| DELETE | `/tags/{id}`                   | Delete a tag                      |

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

## License

This project is licensed under the MIT License — see [LICENSE](LICENSE).
