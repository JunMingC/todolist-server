# Todo Management System

This project is a Todo Management System developed using SQL Server, Entity Framework, and .NET 8.0. It includes a set of tables designed to efficiently manage todo items, their priorities, statuses, and associated tags. The system is built using Test-Driven Development (TDD) principles, with xUnit as the testing framework.

## Table of Contents

1. [Project Overview](#project-overview)
2. [Technologies Used](#technologies-used)
3. [Database Schema](#database-schema)
    - [Priorities Table](#priorities-table)
    - [Status Table](#status-table)
    - [Tags Table](#tags-table)
    - [Todos Table](#todos-table)
    - [TodoTags Junction Table](#todotags-junction-table)
4. [Setup Instructions](#setup-instructions)
5. [Test-Driven Development (TDD)](#test-driven-development-tdd)
6. [Usage](#usage)
7. [Maintenance](#maintenance)
8. [Contributing](#contributing)
9. [License](#license)

## Project Overview

The Todo Management System is designed to help users manage and track their tasks effectively. It leverages SQL Server for data storage and management, Entity Framework for seamless database interactions, and .NET 8.0 for robust application development. The application allows users to categorize tasks by setting priorities, tracking statuses, and assigning tags. Using a Test-Driven Development (TDD) approach with xUnit ensures that the system is reliable and maintainable, providing a solid foundation for future enhancements.

## Technologies Used

-   **SQL Server**: Relational database management system for storing data.
-   **Entity Framework**: An ORM framework for .NET, used to interact with the database.
-   **.NET 8.0**: The framework for building and running the application.
-   **xUnit**: A testing framework for .NET applications, used to implement TDD.

## Database Schema

### Priorities Table

The `Priorities` table stores different priority levels that can be assigned to todo items. It includes the following fields:

-   `Id`: Unique identifier with auto-increment.
-   `Name`: Name of the priority level (e.g., High, Medium, Low).
-   `Color`: Hex codes with alpa values to represent the priority visually
-   `CreatedAt`: Timestamp for when the priority was created (defaults to the current date/time).
-   `UpdatedAt`: Timestamp for when the priority was last updated (defaults to the current date/time).

**Sample Data:**

-   High Priority (`#FF0000`)
-   Medium Priority (`#FFFF00`)
-   Low Priority (`#00FF00`)

### Status Table

The `Status` table stores different statuses that can be assigned to todo items. It includes the following fields:

-   `Id`: Unique identifier with auto-increment.
-   `Name`: Name of the status (e.g., Not Started, In Progress, Completed).
-   `Color`: Hex codes with alpa values to represent the status visually.
-   `CreatedAt`: Timestamp for when the status was created (defaults to the current date/time).
-   `UpdatedAt`: Timestamp for when the status was last updated (defaults to the current date/time).

**Sample Data:**

-   Not Started (`#FFFF00`)
-   In Progress (`#0000FF`)
-   Completed (`#00FF00`)

### Tags Table

The `Tags` table allows categorizing todo items by tags. It includes the following fields:

-   `Id`: Unique identifier with auto-increment.
-   `Name`: Name of the tag (e.g., Personal, Work, Research).
-   `Color`: Hex codes with alpa values to represent the tag visually.
-   `CreatedAt`: Timestamp for when the tag was created (defaults to the current date/time).
-   `UpdatedAt`: Timestamp for when the tag was last updated (defaults to the current date/time).

**Sample Data:**

-   Personal (`#FF69B4`)
-   Work (`#4682B4`)
-   Research (`#8A2BE2`)
-   Development (`#32CD32`)
-   Review (`#FFD700`)
-   Training (`#D3D3D3`)

### Todos Table

The `Todos` table stores individual todo items. It includes the following fields:

-   `Id`: Unique identifier with auto-increment.
-   `Name`: Name of the todo item.
-   `Description`: Description of the todo item (nullable).
-   `DueDate`: Due date for the todo item (nullable).
-   `PriorityId`: Foreign key reference to the `Priorities` table (nullable).
-   `StatusId`: Foreign key reference to the `Status` table (nullable).
-   `CreatedAt`: Timestamp for when the todo item was created (defaults to the current date/time).
-   `UpdatedAt`: Timestamp for when the todo item was last updated (defaults to the current date/time).

**Sample Data:**

-   Prepare project proposal (`High Priority`, `In Progress`)
-   Team meeting (`Medium Priority`, `Not Started`)
-   Code review (`Low Priority`, `Not Started`)

### TodoTags Junction Table

The `TodoTags` table is a junction table that creates a many-to-many relationship between `Todos` and `Tags`. It includes:

-   `TodoId`: Foreign key reference to the `Todos` table.
-   `TagId`: Foreign key reference to the `Tags` table.
-   Composite primary key: `(TodoId, TagId)`.

**Sample Data:**

-   Prepare project proposal linked with `Work` and `Research` tags.
-   Team meeting linked with `Work` and `Training` tags.
-   Code review linked with `Work` and `Research` tags.

## Setup Instructions

1. **Scaffold DbContext from SQL Tables**: Use the Entity Framework tools to scaffold the `DbContext` from existing SQL tables. This can be done using the command:

    ```bash
    dotnet ef dbcontext scaffold "YourConnectionStringHere" Microsoft.EntityFrameworkCore.SqlServer -o Models
    ```

    This command will generate a `DbContext` class and entity classes for each table.

2. **Run Migrations**: Use Entity Framework migrations to apply any additional changes to the database schema:

    ```bash
    dotnet ef migrations add InitialCreate
    dotnet ef database update
    ```

3. **Insert Initial Data**: Modify the `DbContext` or use SQL scripts to seed the database with initial data for the `Priorities`, `Status`, `Tags`, and `Todos` tables.

4. **Run the Application**: Use Visual Studio or the .NET CLI to build and run the application.

## Test-Driven Development (TDD)

This project follows TDD principles using xUnit. To run tests:

1. **Add Tests**: Write unit tests using xUnit to cover different scenarios for managing todo items, priorities, statuses, and tags.
2. **Run Tests**: Execute the tests using the .NET CLI:

    ```bash
    dotnet test
    ```

    Ensure that all tests pass before committing changes.

3. **Continuous Integration**: Integrate tests into a CI pipeline to ensure quality and stability.

## Usage

-   **Adding Todos**: Use the application's interface or API to add todo items with associated priorities, statuses, and tags.
-   **Updating Todos**: Update existing todo items through the interface or API. The `UpdatedAt` timestamp is managed automatically by the system.
-   **Querying**: Use the application's features or direct SQL queries for data retrieval. Entity Framework handles query optimizations.

## Maintenance

-   **Database Updates**: Use Entity Framework migrations to handle schema changes and updates.
