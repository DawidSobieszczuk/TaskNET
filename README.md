# TaskNet API

## Overview

A simple and robust RESTful API for managing a To-Do list, built with .NET 9. The API is fully containerized with Docker and comes with a suite of unit and integration tests.

## Features

- Full CRUD (Create, Read, Update, Delete) operations for tasks.
- Filtering for upcoming tasks (e.g., for Today, Tomorrow, This Week).
- Update task progress with a percentage value.
- Mark tasks as done.
- Built-in data validation for all inputs.

## Tech Stack

- **Backend:** .NET 9, ASP.NET Core
- **Database:** MySQL
- **ORM:** Entity Framework Core
- **Testing:** xUnit, Testcontainers (for integration tests with a real database environment)
- **Containerization:** Docker, Docker Compose

## Getting Started

These instructions are designed to be platform-agnostic and should work on Windows, macOS, and Linux.

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker](https://www.docker.com/get-started)

#### IDE

This project was developed in Visual Studio 2022. You can open the `TaskNET.sln` file to load the project with all the correct settings.

### Running the Application

The easiest way to get the application running is by using Docker Compose.

1.  Clone the repository.
2.  Open your terminal and navigate to the project's root directory (the one containing the `docker-compose.yml` file).
3.  Run the following command:

    ```bash
    docker-compose up --build
    ```

This command will build the .NET application image, start a MySQL container, and run the API. The API will be available at `http://localhost:8080`.

### Running the Tests

To run the full suite of unit and integration tests, navigate to the project's root directory and run:

```bash
dotnet test
```

The integration tests will automatically spin up a dedicated MySQL container using Testcontainers to ensure tests run in an isolated and clean environment.

#### Performance Tests

The project also includes performance benchmarks using BenchmarkDotNet. To run them, use the following command. Note that these tests can take several minutes to complete as they run multiple iterations to get accurate results.

For the most reliable results, always run performance tests in the `Release` configuration.

```bash
dotnet test --filter "Category=Performance" --configuration Release
```

## API Endpoints

| Method | Endpoint                                    | Description                                               |
|--------|---------------------------------------------|-----------------------------------------------------------|
| `GET`    | `/api/tasks`                                | Retrieves all tasks.                                      |
| `GET`    | `/api/tasks/{id}`                           | Retrieves a specific task by its ID.                      |
| `GET`    | `/api/tasks/incoming?incomingTasksFilter={filter}` | Retrieves upcoming tasks. `filter` can be `Today`, `Tomorrow`, or `ThisWeek`. |
| `POST`   | `/api/tasks`                                | Creates a new task.                                       |
| `PUT`    | `/api/tasks/{id}`                           | Updates an existing task.                                 |
| `PATCH`  | `/api/tasks/{id}/percent?percent={value}`   | Sets the completion percentage for a task. `value` is a decimal from 0.0 to 1.0. |
| `PATCH`  | `/api/tasks/{id}/done`                      | Marks a task as complete (sets progress to 100%).         |
| `DELETE` | `/api/tasks/{id}`                           | Deletes a task by its ID.                                 |