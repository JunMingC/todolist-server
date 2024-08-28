# **Todo Management System**

The **Todo Management System** is a robust application designed for managing todo items, built using **SQL Server**, **Entity Framework**, and **.NET 8.0**. This project follows **Test-Driven Development (TDD)** principles and utilizes **xUnit** as the testing framework to ensure code quality and reliability.

## **Live Demo**
Check out the live version hosted on Google Cloud Platform:

- **Link:** [Todo Management System](https://todolist-433816.as.r.appspot.com/)

## **Table of Contents**
1. [Features](#features)
2. [Installation](#installation)
3. [Usage](#usage)
4. [Testing](#testing)
5. [Database Setup](#database-setup)
6. [Technologies Used](#technologies-used)
7. [License](#license)

## **Features**
- **Manage Todos:** Add, update, delete, and view todo items.
- **Prioritization:** Assign priorities to tasks.
- **Status Tracking:** Keep track of task statuses (e.g., pending, completed).
- **Tagging:** Organize tasks with tags for easy filtering.
- **API Documentation:** Swagger UI for easy API testing and exploration.
- **TDD with xUnit:** Comprehensive test coverage using xUnit framework.

## **Installation**
Follow these steps to set up the Todo Management System on your local machine:

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

### Setup Steps
1. **Clone the repository:**
    ```bash
    git clone https://github.com/your-repo-url/todo-management-system.git
    cd todo-management-system
    ```

2. **Update Configuration:**
    - Update the server connection string in:
        - `TodoListApi\.env`
        - `TodoListTest\.env.json`

3. **Populate the Database:**
    - Execute the SQL script provided to set up the database:
        - File: `Scripts/sqlserver/PopulateDatabase.sql`

## **Usage**

1. **Run the API:**
    ```bash
    cd TodoListApi
    dotnet run
    ```
    - Open your browser and navigate to [http://localhost:5228](http://localhost:5228) to access the API.

2. **API Documentation:**
    - Swagger UI will be available at: [http://localhost:5228/swagger/index.html](http://localhost:5228/swagger/index.html)

## **Testing**
To run the automated tests, follow these steps:

1. **Navigate to the test project directory:**
    ```bash
    cd TodoListTest
    ```

2. **Run the tests:**
    ```bash
    dotnet test
    ```

## **Database Setup**
- The system uses **SQL Server** for managing the todo items, priorities, statuses, and tags.
- To initialize the database, use the provided SQL script:
  - Path: `Scripts/sqlserver/PopulateDatabase.sql`

## **Technologies Used**
- **Backend:** .NET 8.0, ASP.NET Core Web API, Entity Framework Core
- **Database:** SQL Server
- **Testing:** xUnit
- **Hosting:** Google Cloud Platform (GCP)
- **API Documentation:** Swagger