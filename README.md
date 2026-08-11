# Project Management Application

A simple **Project Management and Task Tracking System** built using **ASP.NET Core MVC, C#, Entity Framework Core, and SQL Server**.

## Features

* Project management
* Project members
* Board management
* Task management
* Task assignment
* Sprint management
* Kanban board
* Comments
* File attachments
* Notifications
* Dashboard with project statistics

## Technologies

* C#
* ASP.NET Core MVC
* Entity Framework Core
* SQL Server
* HTML / CSS
* JavaScript
* Bootstrap

## Architecture

The project follows a layered structure:

```text
ProjectTracker
│
├── ProjectTracker.Models
├── ProjectTracker.Core
└── ProjectTracker.Web
```

It uses:

* Repository Pattern
* Service Layer
* DTOs
* Dependency Injection
* OOP and SOLID principles

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/AnzalnaNazar0896/Project-Management-Application.git
```

### 2. Configure the database

Update the SQL Server connection string in:

```text
appsettings.json
```

### 3. Update the database

```bash
dotnet ef database update
```

### 4. Run the application

```bash
dotnet run
```

Or open the solution in **Visual Studio** and run the project.

## Project Workflow

```text
Project
   ↓
Board
   ↓
Tasks
   ↓
Sprint
   ↓
Kanban Board
   ↓
Task Completion
```

## Author

**Anzalna Nazar**

GitHub: AnzalnaNazar0896
