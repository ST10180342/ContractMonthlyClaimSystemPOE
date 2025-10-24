Overview
This is a .NET Core 8.0 ASP.NET Core MVC web application designed for lecturers, programme coordinators, and academic managers to handle monthly claims efficiently. Built with Entity Framework Core for data persistence, ASP.NET Core Identity for role-based authentication, and SignalR for real-time status updates, it provides a secure, intuitive GUI for claim submission, approval, and tracking.
The app supports three roles:

Lecturer: Submit claims and view personal status.
Coordinator/Manager: Review and approve/reject pending claims.

Key technologies:

Backend: C#, Entity Framework Core (SQL Server/LocalDB), ASP.NET Core Identity.
Frontend: Bootstrap 4.5 for responsive UI, Razor Views.
Testing: xUnit with Moq for unit tests.
Real-time: SignalR for status notifications.

Features
The application implements the following core functionalities:

Lecturers can submit their claims at any time with a click of a button:

A simple, form in /Claims/Submit with fields for hours worked, hourly rate, and additional notes.
Prominent blue "Submit Claim" button for easy user flow.
Layout uses a calm blue color scheme (Bootstrap primary) with validation and success messages.

Programme Coordinators and Academic Managers can easily verify and approve the claims:

Separate view at /Claims/PendingClaims displaying pending claims in organized cards.
Each card shows details (hours, rate, total, notes, submitted date, document link).

Lecturers can upload supporting documents for their claims:

"Upload" button in the submission form (accepts PDF, DOCX, XLSX only).
Files are securely stored in wwwroot/uploads with unique names (GUID + extension).
5MB size limit; invalid uploads throw user-friendly errors.
Uploaded file name displays on the form; linked in claim views for download.

The claim status can be tracked transparently until it is settled:

Status labels (Pending/Warning, Approved/Success, Rejected/Danger) and progress bars in /Claims/MyClaims.
Real-time updates via SignalR: On approval/rejection, lecturers get push notifications in their views (connect to /claimHub).
Updates persist in DB and reflect immediately on refresh.

The system always provides consistent and reliable information:

Unit testing: xUnit tests in ContractMonthlyClaimSystemPOE.Tests cover submission, validation, approval, and mocks for DbContext/UserManager.
Error handling: Try-catch in actions with ModelState.AddModelError for validation; TempData for user messages (success/error alerts).
Graceful failures: Invalid files/models show meaningful messages; DB errors log and redirect.

Prerequisites

.NET 8.0 SDK
Visual Studio 2022 (Community free) or VS Code with C# extension
SQL Server LocalDB (included with VS) or SQL Server Express
Git (for cloning/collaborating)

Installation & Setup

Clone the Repository:
textgit clone https://github.com/st10180342/ContractMonthlyClaimSystemPOE.git
cd ContractMonthlyClaimSystemPOE

Access Features:

Lecturer: /Claims/Submit (form/upload), /Claims/MyClaims (track status).
Coordinator/Manager: /Claims/PendingClaims (review/approve).
Real-time: Status updates push via SignalR (open two browsers to test).

Project Structure
textContractMonthlyClaimSystemPOE/
├── Controllers/          # MVC controllers (ClaimsController.cs)
├── Data/                 # DbContext (ApplicationDbContext.cs)
├── Hubs/                 # SignalR (ClaimHub.cs)
├── Models/               # Entities (Claim.cs, ApplicationUser.cs)
├── Views/                # Razor views (Shared/_Layout.cshtml, Claims/Submit.cshtml, etc.)
├── wwwroot/uploads/      # Uploaded files (auto-created)
├── Program.cs            # App startup & config
├── appsettings.json      # Connection strings & settings
├── ContractMonthlyClaimSystemPOE.csproj  # Main project
├── ContractMonthlyClaimSystemPOE.sln     # Solution
└── Tests/                # Unit tests
    └── ContractMonthlyClaimSystemPOE.Tests.csproj
        └── ClaimsControllerTests.cs

Known Issues & Improvements

File Storage: Local uploads—migrate to Azure Blob for production.
Real-Time: SignalR JS client needed in views for full push updates (add to MyClaims.cshtml).
Security: Add HTTPS enforcement; validate roles on all actions.
DB: Run migrations on deploy (e.g., Azure SQL)

Built with ❤️ using .NET 8.0. Questions? Open an issue on GitHub.
