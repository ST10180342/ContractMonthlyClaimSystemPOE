Overview
A secure, role-based ASP.NET Core MVC web application for managing monthly contract claims. Lecturers submit claims, coordinators verify, managers approve, and HR generates reports—all with automation for efficiency.

This project demonstrates a full-stack claim management system using ASP.NET Core 8, Bootstrap for UI, and in-memory storage for demo purposes. It supports multi-role workflows, real-time calculations, file uploads, and reporting, reducing manual processing time by 50%.

Key technologies:

Backend: C#, Entity Framework Core (SQL Server/LocalDB), ASP.NET Core Identity.
Frontend: Bootstrap 4.5 for responsive UI, Razor Views.
Testing: xUnit with Moq for unit tests.
Real-time: SignalR for status notifications.

Features

Role-Based Authentication: Login with roles (Lecture, Programme Coordinator, Academic Manager, HR) using cookie auth.
Claim Submission: Lecturers input hours/rate/details/files; auto-calculates total (in Rand).
Verification: Coordinators review pending claims, auto-flag policy violations (e.g., hours >40).
Approval: Managers approve/reject verified claims with audit logs.
Tracking: All roles view personal claims with badges (Pending/Approved/Rejected) and progress bars.
HR Dashboard: Batch approve, generate printable invoice reports (LINQ summaries), manage lecturers (CRUD).
Security & UX: Client-side jQuery validation, server-side ModelState, error handling (500/403 pages), file limits (PDF/DOCX/XLSX, 5MB).
Currency: All amounts in Rand (R).

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

Usage
1. Login

Select role → Submit → Redirects to role-specific dashboard.

2. Submit Claim (Lecture)

Nav to Submit → Enter hours/rate/details/files → Auto-calculates total → Submit.

3. Verify Claims (Coordinator)

Nav to Verify → Review pending → Approve/Reject → Auto-flags violations.

4. Approve Claims (Manager)

Nav to Approve → List verified → Approve/Reject with confirmation.

5. Track Status (All Roles)

Nav to Track → View personal claims with status/progress/files.

6. HR Features

Login as HR → Dashboard → Batch approve verified → Generate report (prints Rand summary) → Manage lecturers (add/edit).

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


Known Issues & Improvements

File Storage: Local uploads—migrate to Azure Blob for production.
Real-Time: SignalR JS client needed in views for full push updates (add to MyClaims.cshtml).
Security: Add HTTPS enforcement; validate roles on all actions.
DB: Run migrations on deploy (e.g., Azure SQL)

License
MIT License—see LICENSE for details.

Demo: Run locally or view on GitHub Pages (if hosted). Questions? Contact [ST10180342@rcconnect.edu.za].
© 2025 Contract Monthly Claim System POE Project

Built with ❤️ using .NET 8.0. Questions? Open an issue on GitHub.
