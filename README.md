# Smart Student Management System

A RESTful backend API for managing students, teachers, classes, courses, enrollments, grades, and attendance.

## Project Overview

The Smart Student Management System provides a centralized backend for academic administration. It exposes REST endpoints through ASP.NET Core Web API and stores application data in SQL Server using Entity Framework Core.

**Project scope:** Backend API only. No frontend application is included.

## Main Features

- Student registration and JWT-based login.
- Role-based access control for Admin, Teacher, and Student.
- Student profile and class management.
- Course management and teacher assignment.
- Student-course enrollment with duplicate-enrollment prevention.
- Grade creation and updates by the teacher assigned to the course.
- Attendance recording and updates by the teacher assigned to the course.
- Student access to their own grades and attendance history.
- Administrative user management and system statistics.
- Swagger/OpenAPI documentation.

## Technologies

- ASP.NET Core Web API (.NET 10)
- C#
- Entity Framework Core
- SQL Server LocalDB
- ASP.NET Core Identity
- JWT Bearer Authentication
- Swagger / OpenAPI
- Visual Studio

## User Roles

| Role | Main Permissions |
|---|---|
| Admin | Manage students, teachers, classes, courses, enrollments, roles, and statistics |
| Teacher | Manage grades and attendance for assigned courses |
| Student | View their own grades and attendance |

## Database

The database includes the academic entities `Students`, `Classes`, `Courses`, `StudentCourses`, `Grades`, and `Attendances`, together with ASP.NET Core Identity tables.

The `StudentCourses` table represents the many-to-many relationship between students and courses.

See [`Documentation/Database_ERD.png`](Documentation/Database_ERD.png) for the database diagram.

## Getting Started

### Prerequisites

- .NET 10 SDK
- Visual Studio with ASP.NET and web development workload
- SQL Server LocalDB

### Configuration

1. Clone this repository.
2. Open `SmartStudentManagement.slnx` in Visual Studio.
3. Configure the database connection string and JWT settings using local User Secrets or environment variables.
4. Do not commit JWT signing keys, passwords, or other secrets to source control.
5. Apply the Entity Framework Core migrations to the configured database.
6. Run the API project from Visual Studio.

The development connection string used during this project was based on SQL Server LocalDB. Update it for your own machine if necessary.

### Run the API

Run the `SmartStudentManagement` project in Visual Studio. Open the Swagger UI at:

`https://localhost:<your-https-port>/swagger`

The HTTPS port may differ according to your local launch settings.

## API Documentation and Supporting Files

The `Documentation` folder contains:

- `SmartStudent_API_Documentation.docx` — API endpoint and request-schema documentation.
- `SmartStudent_Postman_Collection.json` — Postman collection generated from the OpenAPI specification. Set the collection variables such as `baseUrl`, `token`, and entity IDs before use.
- `Database_ERD.png` — database entity relationship diagram.
- `SmartStudent_Project_Report.docx` — project report.

## Security Notes

- JWT signing keys and local development secrets must be stored outside tracked source files.
- `appsettings.Development.json`, build output, and Visual Studio user-specific files are excluded through `.gitignore`.
- Replace development credentials and review all configuration before deploying to production.

## Testing

During development, representative functional, validation, authorization, ownership, and deletion-protection scenarios were exercised through Swagger. This repository does not claim a complete automated test suite or formal load testing.

## Future Improvements

- Add automated unit and integration tests.
- Expand OpenAPI response documentation.
- Add centralized structured logging and exception handling.
- Add pagination, filtering, and sorting for collection endpoints.
- Prepare production deployment and CI/CD configuration.
