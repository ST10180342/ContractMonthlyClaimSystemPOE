using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ContractMonthlyClaimSystemPOE.Models; // For Claim model
using System.Security.Claims; // For ClaimTypes (fully qualify if ambiguous)
using Microsoft.AspNetCore.Http; // For IFormFile
using System.IO;
using System.Linq; // For Any() and Where()
using System.Collections.Generic; // For List
using System; // For Guid and DateTime

namespace ContractMonthlyClaimSystemPOE.Controllers
{
    [Authorize] // Requires login for all actions in this controller
    public class ClaimController : Controller
    {
        // In-memory storage for demo (replace with DB in production) - Use Models.Claim
        internal static List<ContractMonthlyClaimSystemPOE.Models.Claim> _claims = new List<ContractMonthlyClaimSystemPOE.Models.Claim>(); // Changed to internal for access
        internal static int _nextId = 1; // Also internal for access

        public IActionResult Index()
        {
            // Simple dashboard: Show welcome message based on role - Fully qualify auth Claim
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "User";
            ViewBag.Role = role;
            ViewBag.Username = User.Identity?.Name ?? "Guest";
            return View();
        }

        // Submit Claim: GET (show form)
        [Authorize(Roles = "Lecture,ProgrammeCoordinator,AcademicManager")]
        public IActionResult Submit()
        {
            // Pre-fill lecturer name from user if possible
            var model = new ContractMonthlyClaimSystemPOE.Models.Claim { LecturerName = User.Identity?.Name ?? "Unknown" };
            return View(model);
        }

        // Submit Claim: POST (process form)
        [HttpPost]
        [Authorize(Roles = "Lecture,ProgrammeCoordinator,AcademicManager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(ContractMonthlyClaimSystemPOE.Models.Claim model, List<IFormFile>? supportingDocuments)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model); // Return form with errors
                }

                // Auto-calculate amount
                model.Amount = model.HoursWorked * model.HourlyRate;

                // Handle file uploads with validation
                if (supportingDocuments != null && supportingDocuments.Any())
                {
                    var allowedTypes = new[] { ".pdf", ".docx", ".xlsx" };
                    const long maxFileSize = 5 * 1024 * 1024; // 5MB

                    model.Files ??= new List<string>();
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    Directory.CreateDirectory(uploadsFolder);

                    foreach (var file in supportingDocuments)
                    {
                        if (file.Length > 0)
                        {
                            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                            if (!allowedTypes.Contains(extension))
                            {
                                ModelState.AddModelError("SupportingDocuments", $"Invalid file type: {file.FileName}. Allowed: PDF, DOCX, XLSX.");
                                return View(model);
                            }
                            if (file.Length > maxFileSize)
                            {
                                ModelState.AddModelError("SupportingDocuments", $"File too large: {file.FileName} ({file.Length / 1024} KB). Max: 5MB.");
                                return View(model);
                            }

                            var fileName = Guid.NewGuid().ToString() + extension;
                            var filePath = Path.Combine(uploadsFolder, fileName);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                            model.Files.Add(fileName); // Securely link to claim
                        }
                    }
                }

                // Set submitter (from user or manual) - Prefer manual if provided
                model.LecturerName = !string.IsNullOrEmpty(model.LecturerName) ? model.LecturerName : (User.Identity?.Name ?? "Unknown");

                // Set claim month if not provided (default to current month)
                if (model.ClaimMonth == null)
                {
                    model.ClaimMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                }

                model.Id = _nextId++;
                model.SubmittedDate = DateTime.Now;
                _claims.Add(model);

                TempData["Success"] = $"Claim submitted successfully! Total: R{model.Amount:F2}";
                return RedirectToAction("Index"); // Or "Track" to show submitted claim
            }
            catch (IOException ex)
            {
                TempData["Error"] = "File upload failed—disk space or permissions issue. Try fewer files.";
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Submission failed due to an unexpected error. Please try again.";
                // Log ex in prod
                return View(model);
            }
        }

        // Verify Claims: GET (list pending)
        [Authorize(Roles = "ProgrammeCoordinator,AcademicManager")]
        public IActionResult Verify()
        {
            var pendingClaims = _claims.Where(c => c.Status == "Pending").ToList();
            return View(pendingClaims);
        }

        // Approve Claims: GET (list claims ready for approval)
        [Authorize(Roles = "AcademicManager")]
        public IActionResult Approve()
        {
            var approvableClaims = _claims.Where(c => c.Status == "Verified" || c.Status == "Pending").ToList();
            return View(approvableClaims);
        }

        // Approve Claim: POST
        [HttpPost]
        [Authorize(Roles = "ProgrammeCoordinator,AcademicManager")]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(int id)
        {
            try
            {
                var claim = _claims.FirstOrDefault(c => c.Id == id);
                if (claim != null && claim.Status == "Pending")
                {
                    claim.Status = "Approved";
                    claim.VerifiedBy = User.Identity?.Name;
                    claim.VerifiedDate = DateTime.Now;
                    TempData["Success"] = "Claim submitted successfully! Total: R {model.Amount:F2}";
                }
                else
                {
                    TempData["Error"] = "Claim not found or already processed.";
                }
            }
            catch (InvalidOperationException) // e.g., concurrent modification
            {
                TempData["Error"] = "Approval failed—another user may have updated this claim. Refresh to check.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred during approval. Status may not have updated.";
                // Log ex
            }
            return RedirectToAction("Verify");
        }

        // Reject Claim: POST
        [HttpPost]
        [Authorize(Roles = "ProgrammeCoordinator,AcademicManager")]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(int id)
        {
            try
            {
                var claim = _claims.FirstOrDefault(c => c.Id == id);
                if (claim != null && claim.Status == "Pending")
                {
                    claim.Status = "Rejected";
                    claim.VerifiedBy = User.Identity?.Name;
                    claim.VerifiedDate = DateTime.Now;
                    TempData["Success"] = $"Claim {id} rejected. Total: R {{claim.Amount:F2}}\"";
                }
                else
                {
                    TempData["Error"] = "Claim not found or already processed.";
                }
            }
            catch (InvalidOperationException)
            {
                TempData["Error"] = "Rejection failed—another user may have updated this claim. Refresh to check.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred during rejection. Status may not have updated.";
                // Log ex
            }
            return RedirectToAction("Verify");
        }

        // Track Status: GET (user's own claims)
        [Authorize(Roles = "Lecture,ProgrammeCoordinator,AcademicManager")]
        public IActionResult Track()
        {
            var username = User.Identity?.Name ?? "Unknown";
            var userClaims = _claims.Where(c => c.LecturerName == username).ToList();
            return View(userClaims);
        }
    }
}
