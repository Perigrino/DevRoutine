# Contributing to DevRoutine

Thanks for your interest in contributing! This guide covers how to get set up and how to contribute code.

## Getting Started

1. Fork the repository and clone your fork.
2. Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).
3. Start the backing services with Docker: `docker compose up -d`.
4. Build the solution: `dotnet build DevRoutine.sln`.
5. Run the tests: `dotnet test DevRoutine.sln`.

## Development Workflow

- Create a branch off `master` for your work: `git checkout -b my-feature`.
- Keep changes focused and scoped to a single concern.
- Run `dotnet build DevRoutine.sln` and `dotnet test DevRoutine.sln` before opening a pull request.
- The project treats warnings as errors and enables strict analyzers (including SonarAnalyzer) for the API project—please keep the build warning-free.

## Code Style

- Follow the existing conventions: file-scoped namespaces, primary constructors, records for DTOs, and FluentValidation for input validation.
- Respect the settings in `Directory.Build.props` and `.editorconfig`.
- Add or update tests in `tests/DevRoutine.Api.Tests` for any behavior you change or add.

## Pull Requests

- Describe the motivation and the change in the PR description.
- Reference any related issue.
- Ensure CI passes (the build workflow restores, builds, tests, and publishes the solution).

## Reporting Issues

- Include steps to reproduce, expected behavior, and the environment (OS, .NET version, Docker version) in bug reports.
