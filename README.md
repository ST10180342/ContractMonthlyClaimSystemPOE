# Contract Monthly Claim System
ASP.NET Core MVC web app for lecturers to submit claims, coordinators/managers to verify/approve, and track status.

## Features
- Submit claims with hours, rate, and file uploads.
- Role-based access (Lecturer, Programme Coordinator, Academic Manager).
- Verify/approve claims and track status.
- Uses Bootstrap 4.5.2 for responsive UI and EF Core for database.

## Structure
- `Startup.cs`: Configures MVC, routing, HTTPS.
- `Models/Claim.cs`: Defines claim data (Id, LecturerName, etc.).
- `Controllers/`: HomeController (Index, Error), ClaimController (Submit, Verify, Approve, Track).
- `Views/`: Login, Error, and layout pages.

## Setup
- Install .NET Core SDK, run with `dotnet run`.
