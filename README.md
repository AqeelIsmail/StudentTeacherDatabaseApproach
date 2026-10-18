# Student–Teacher Web API — EF Core Database-First Approach

**PROG6212 Programming 2B | Student support example | ASP.NET Core Web API (.NET 10)**

This repository demonstrates how to develop a RESTful ASP.NET Core Web API using **Entity Framework Core Database First (reverse engineering)** and an existing **SQL Server** database. It is a learning resource for understanding database connections, scaffolding, dependency injection, authentication, authorization and API testing.

> **Important for the RaceDay POE:** This is a **Student–Teacher example, not the RaceDay solution**. Use it to understand the workflow, then implement your own RaceDay entities, endpoints and rules from your approved Part 1 ERD, SQL script and API endpoint plan. Students may use Code First or Database First as communicated by the lecturer; all other Part 2 requirements, including unit tests and CI/CD, remain applicable.

## Repository contents

```text
StudentTeacherDatabaseApproach/
├── README.md                         # These instructions
├── StudentTeacher.sql                # Creates and seeds StudentTeacherDB
├── .gitignore
└── StudentTeacherDatabaseApproach/
    ├── Controllers/                  # API endpoints and role restrictions
    ├── Data/                         # Scaffolded StudentTeacherDbContext
    ├── DTOs/                         # API request/response shapes
    ├── Models/                       # Seven scaffolded entity classes
    ├── Properties/launchSettings.json
    ├── appsettings.json
    ├── Program.cs
    ├── StudentTeacherDatabaseApproach.csproj
    └── StudentTeacherDatabaseApproach.slnx
```

## Prerequisites

- **Visual Studio 2026**, with the **ASP.NET and web development** workload and **.NET 10 SDK** installed.
- A working Microsoft **SQL Server Express** instance (`localhost\SQLEXPRESS`) or **SQL Server Express LocalDB** instance (`(localdb)\MSSQLLocalDB`).
- **SQL Server Management Studio (SSMS)**, recommended for running the SQL script and inspecting tables; Visual Studio SQL Server Object Explorer is an alternative where supported.
- Git (optional if you download the repository as a ZIP).

**SQL Server** is the database engine. **SSMS** is a graphical tool for connecting to it. **LocalDB** and **SQLEXPRESS** are different local SQL Server instance options; either can be used with EF Core Code First or Database First.

## Quick start — run the supplied API

### 1. Download the project

Select **Code → Download ZIP** on GitHub and extract it, or clone the repository:

```bash
git clone <YOUR-GITHUB-REPOSITORY-URL>
```

Open `StudentTeacherDatabaseApproach/StudentTeacherDatabaseApproach.slnx` in Visual Studio. (Alternatively, open the `.csproj` directly.)

### 2. Create the SQL Server database

1. Open **SSMS** and connect to your working SQL Server instance using **Windows Authentication**.
2. Open the root-level `StudentTeacher.sql` file using **File → Open → File**.
3. Check that you are connected to the correct instance, then select **Execute**.
4. Refresh **Databases** and confirm that `StudentTeacherDB` exists.
5. Expand **Tables** and confirm that these seven tables were created:

```text
Users
Student_Profiles
Teacher_Profiles
Course_Categories
Courses
Enrolments
Course_Results
```

The script includes sample records so you can inspect the relationships. **Run the creation script once on a new local database.** It contains `CREATE DATABASE` and `CREATE TABLE` commands, and rerunning it against an existing database may fail. Do not delete an existing database containing work you wish to keep.

### 3. Set your connection string

In the Web API project, open `appsettings.json` and update `ConnectionStrings:DefaultConnection` to match **your SQL Server instance**.

**If you use SQL Server Express:**

```json
"DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=StudentTeacherDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

**If you use LocalDB:**

```json
"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=StudentTeacherDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

In JSON, `\\` represents one backslash in the server name. Both examples use **Windows Authentication** (`Trusted_Connection=True`). `TrustServerCertificate=True` is used here for local development; do not assume it is appropriate for production systems.

> If the database is on a different instance, use its actual server name. Copying the lecturer's connection string does **not** install that SQL Server instance or create the database on your computer.

### 4. Restore packages and build

In Visual Studio:

1. Right-click the solution and choose **Restore NuGet Packages** if restore has not already happened automatically.
2. Set `StudentTeacherDatabaseApproach` as the **Startup Project**.
3. Select **Build → Build Solution** (`Ctrl+Shift+B`).

The `.csproj` already lists the required packages, including the SQL Server EF Core provider, EF Core design/tools and Swagger. **You do not need to install each package again when opening this existing project**; NuGet restore obtains the declared packages.

### 5. Run Swagger UI

Choose the **https** launch profile and press **F5**. The included `launchSettings.json` is configured to open:

```text
https://localhost:7244/swagger
```

The HTTP profile uses `http://localhost:5243/swagger`. Keep the actual URLs displayed by Visual Studio if the port numbers change. If the browser opens at `/`, append `/swagger`.

Swagger lists the available controller endpoints. If it does not load, confirm the application is running in the `Development` environment and check `Program.cs` for `AddSwaggerGen()`, `UseSwagger()` and `UseSwaggerUI()`.

### 6. Confirm that the API communicates with SQL Server

1. In Swagger, try a read endpoint such as `GET /api/Categories` where access is permitted. An empty list is possible; a SQL connection exception indicates the database setup must be checked.
2. Use `POST /api/Auth/register` to register a **new** Student with a unique email and student number. Follow the field names displayed in Swagger.
3. Confirm the new record exists using SSMS:

```sql
USE StudentTeacherDB;
SELECT * FROM Users;
SELECT * FROM Student_Profiles;
```

4. Use `POST /api/Auth/login` with the newly registered credentials. Swagger runs in the browser and can retain the authentication cookie for subsequent same-origin requests.
5. Test a Student-protected endpoint, then try a Teacher-only endpoint to observe role restrictions. Register and log in as a Teacher to test the Teacher functions, course creation and enrolment management.
6. Test logout and verify access to protected endpoints is no longer permitted.

**Do not assume the seeded accounts have known passwords.** Register fresh test accounts instead. Use different email addresses and student/employee numbers for each test.

## How Database First was set up (for learning)

The repository already includes the generated models and `StudentTeacherDbContext`, so **you do not need to scaffold again just to run it**. If you want to reproduce the Database-First setup in a fresh project, follow this sequence:

1. In Visual Studio, create an **ASP.NET Core Web API** project targeting **.NET 10**, with **controllers enabled**, **OpenAPI support enabled** and **Authentication type: None**.
2. Create `StudentTeacherDB` first by running `StudentTeacher.sql` in SQL Server.
3. From **Tools → NuGet Package Manager → Package Manager Console**, select the **Web API project** as the Default project, then install:

```powershell
Install-Package Microsoft.EntityFrameworkCore.SqlServer -Version 10.0.13
Install-Package Microsoft.EntityFrameworkCore.Tools -Version 10.0.13
Install-Package Microsoft.EntityFrameworkCore.Design -Version 10.0.13
```

4. With the correct server name and existing database, run:

```powershell
Scaffold-DbContext "Server=localhost\SQLEXPRESS;Database=StudentTeacherDB;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -ContextDir Data -Context StudentTeacherDbContext -NoOnConfiguring
```

5. EF Core creates the entity classes inside `Models/` and a `StudentTeacherDbContext` inside `Data/`. Add the connection string to `appsettings.json`, then register the context in `Program.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using StudentTeacherDatabaseApproach.Data;

builder.Services.AddDbContext<StudentTeacherDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
```

The snippet above belongs **inside the startup configuration after creating `builder`**; the supplied API already contains this registration. Add controllers, cookie authentication, authorization, Swagger, and route mappings to make the API functional. Scaffolding **does not generate your API controllers, DTOs or authentication logic**.

**Important:** In the *existing* repository, do not re-run `Scaffold-DbContext` over generated code unless you intentionally need to update the database model. Re-scaffolding with `-Force` can overwrite generated files; keep custom application logic outside generated files. Do not run `Add-Migration` or `Update-Database` just to set up this existing Database-First example.

## How the project is organised

| Folder/file | Purpose |
|---|---|
| `Models/` | EF Core entity classes generated from the existing SQL tables |
| `Data/StudentTeacherDbContext.cs` | EF Core database context, table mappings, keys and relationships |
| `DTOs/` | Defines accepted/requested API data without exposing full database entities |
| `Controllers/` | Handles API requests, database operations and role restrictions |
| `Program.cs` | Registers services, `DbContext`, cookie authentication, authorization, Swagger and routes |
| `appsettings.json` | Stores local configuration, including the SQL Server connection string |
| `StudentTeacher.sql` | Creates and seeds the reference database |

## Common problems

| Problem | What to check |
|---|---|
| `Cannot open database 'StudentTeacherDB'` | SQL script executed on the same SQL Server instance named in `appsettings.json`, and Windows login has access |
| Cannot connect to `localhost\SQLEXPRESS` | Check whether SQLEXPRESS is installed/running; if you use LocalDB, change `Server` accordingly |
| Swagger does not appear | Startup project, HTTPS profile, `launchUrl: "swagger"`, Development environment and Swagger middleware |
| `Scaffold-DbContext` fails | Database already exists and is accessible; correct provider, EF packages and Default project selected |
| Existing scaffolded files would be overwritten | Do not re-scaffold merely to run this supplied example |
| A Student cannot access a Teacher-only endpoint | Expected role-based authorization; log in as a Teacher to test it |

## Relation to the RaceDay POE

This project is **supporting learning material**, not an assessment submission. In your own RaceDay project, use the **entities, constraints and endpoints from your approved Part 1 plan**, not Student–Teacher names. Whether you choose Code First or Database First (as permitted by the lecturer's additional instructions), your Part 2 work must still include the required API features, role/session authentication, password hashing, Swagger documentation, **unit tests**, **GitHub Actions CI/CD**, your own meaningful commits, and the README/video evidence required by the POE.

The lecturer's example does not need to include a test project for you to understand Database First. However, **your own assessed RaceDay Part 2 does require tests**. Do not copy this API or its code as your own assessment work without appropriate disclosure and acknowledgement under your module's academic-integrity rules.

## Further reading

- [Microsoft — Reverse Engineering (Scaffolding) in EF Core](https://learn.microsoft.com/en-us/ef/core/managing-schemas/scaffolding/)
- [Microsoft — EF Core Package Manager Console tools](https://learn.microsoft.com/en-us/ef/core/cli/powershell)
- [Microsoft — SQL Server Express LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)
- [Strapi — Code First vs Database First](https://strapi.io/blog/code-first-vs-database-first)
