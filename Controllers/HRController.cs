using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ContractMonthlyClaimSystemPOE.Models;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace ContractMonthlyClaimSystemPOE.Controllers
{
    [Authorize(Roles = "HR")] // HR = Manager for demo
    public class HRController : Controller
    {
        // Shared from ClaimController (internal access)
        private static List<Claim> _claims = ClaimController._claims;
        private static List<Lecturer> _lecturers = new List<Lecturer>(); // In-memory for lecturers
        private static int _nextLecturerId = 1;

        // Simple DTO for invoice summary (fixes anonymous type issue)
        public class ClaimSummary
        {
            public string Lecturer { get; set; } = string.Empty;
            public string MonthYear { get; set; } = string.Empty;
            public double TotalAmount { get; set; }
            public int ClaimCount { get; set; }
        }

        public IActionResult Index()
        {
            var verifiedClaims = _claims.Where(c => c.Status == "Verified").ToList();
            ViewBag.LecturerCount = _lecturers.Count;
            return View(verifiedClaims);
        }

        // Automated Invoice/Report Generation
        [HttpPost]
        public IActionResult GenerateInvoice()
        {
            var approvedClaims = _claims.Where(c => c.Status == "Approved").ToList();
            if (!approvedClaims.Any())
            {
                TempData["Error"] = "No approved claims to report.";
                return RedirectToAction("Index");
            }

            // LINQ automation: Summarize by lecturer/month (using DTO)
            var summary = approvedClaims
                .GroupBy(c => new { c.LecturerName, Month = c.ClaimMonth.Value.Month, Year = c.ClaimMonth.Value.Year })
                .Select(g => new ClaimSummary
                {
                    Lecturer = g.Key.LecturerName,
                    MonthYear = $"{g.Key.Month}/{g.Key.Year}",
                    TotalAmount = g.Sum(c => c.Amount),
                    ClaimCount = g.Count()
                })
                .OrderBy(s => s.Lecturer)
                .ToList();

            var totalPayment = summary.Sum(s => s.TotalAmount); // double to match Amount type

            // Generate HTML "invoice" (printable)
            var html = GenerateInvoiceHtml(summary, totalPayment);
            ViewBag.InvoiceHtml = html;
            ViewBag.TotalPayment = totalPayment; // Pass to view
            TempData["Success"] = $"Generated report for {approvedClaims.Count} approved claims. Total: ${totalPayment:F2}.";
            return View("InvoiceReport", summary); // New view below
        }

        private string GenerateInvoiceHtml(List<ClaimSummary> summary, double totalPayment)
{
    var sb = new StringBuilder();
    sb.Append("<div class='invoice-report' style='font-family: Arial; max-width: 800px; margin: 0 auto; padding: 20px; border: 1px solid #ddd;'>");
    sb.Append("<h2 style='text-align: center;'>Monthly Claim Invoice Summary</h2>");
    sb.Append($"<p style='text-align: center;'>Generated on {DateTime.Now:MMMM dd, yyyy} | Total Payment: <strong>R {totalPayment:F2}</strong></p>");
    sb.Append("<table style='width: 100%; border-collapse: collapse; margin-top: 20px;'>");
    sb.Append("<thead><tr style='background-color: #f2f2f2;'><th>Lecturer</th><th>Month/Year</th><th>Claims</th><th>Amount (R)</th></tr></thead>");
    sb.Append("<tbody>");
    foreach (var item in summary)
    {
        sb.Append($"<tr><td>{item.Lecturer}</td><td>{item.MonthYear}</td><td>{item.ClaimCount}</td><td>R {item.TotalAmount:F2}</td></tr>");
    }
    sb.Append("</tbody></table>");
    sb.Append("<p style='text-align: right; margin-top: 20px;'>Approved for Payment</p>");
    sb.Append("</div>");
    sb.Append("<script>window.print();</script>"); // Auto-print on load
    return sb.ToString();
}
        // Batch Approve All Verified (automation)
        [HttpPost]
        public IActionResult BatchApprove()
        {
            var verifiedClaims = _claims.Where(c => c.Status == "Verified").ToList();
            if (!verifiedClaims.Any())
            {
                TempData["Error"] = "No verified claims to process.";
                return RedirectToAction("Index");
            }

            foreach (var claim in verifiedClaims)
            {
                claim.Status = "Approved";
                claim.VerifiedBy = "HR Batch";
                claim.VerifiedDate = DateTime.Now;
            }
            TempData["Success"] = $"Batch approved {verifiedClaims.Count} claims.";
            return RedirectToAction("Index");
        }

        // Lecturer Management: List
        public IActionResult ManageLecturers()
        {
            return View(_lecturers);
        }

        // Add Lecturer
        [HttpPost]
        public IActionResult AddLecturer(Lecturer lecturer)
        {
            if (!ModelState.IsValid)
            {
                return View("ManageLecturers", _lecturers);
            }
            lecturer.Id = _nextLecturerId++;
            lecturer.UpdatedDate = DateTime.Now;
            _lecturers.Add(lecturer);
            TempData["Success"] = $"Added lecturer: {lecturer.Name}.";
            return RedirectToAction("ManageLecturers");
        }

        // Edit Lecturer
        [HttpGet]
        public IActionResult EditLecturer(int id)
        {
            var lecturer = _lecturers.FirstOrDefault(l => l.Id == id);
            if (lecturer == null)
            {
                TempData["Error"] = "Lecturer not found.";
                return RedirectToAction("ManageLecturers");
            }
            return View(lecturer);
        }

        [HttpPost]
        public IActionResult EditLecturer(Lecturer lecturer)
        {
            if (!ModelState.IsValid)
            {
                return View(lecturer);
            }
            var existing = _lecturers.FirstOrDefault(l => l.Id == lecturer.Id);
            if (existing == null)
            {
                TempData["Error"] = "Lecturer not found.";
                return RedirectToAction("ManageLecturers");
            }
            existing.Email = lecturer.Email;
            existing.Phone = lecturer.Phone;
            existing.UpdatedDate = DateTime.Now;
            TempData["Success"] = $"Updated {lecturer.Name}.";
            return RedirectToAction("ManageLecturers");
        }
    }
}
