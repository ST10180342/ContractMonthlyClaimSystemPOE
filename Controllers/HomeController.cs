using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ContractMonthlyClaimSystemPOE.Models; // Keep for LoginViewModel
using System.Security.Claims; // For ClaimTypes and ClaimsPrincipal
using System.Collections.Generic; // For List
using System.Threading.Tasks;

namespace ContractMonthlyClaimSystemPOE.Controllers
{
    public class HomeController : Controller
    {
        // GET: Show login form
        public IActionResult Index()
        {
            return View();
        }

        // POST: Handle login
        [HttpPost]
        [ValidateAntiForgeryToken] // Security: Prevent CSRF
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model); // Return to login with errors
            }

            // Demo mode: Accept ANY non-empty username/password + valid role
            if (!string.IsNullOrWhiteSpace(model.Username) &&
                !string.IsNullOrWhiteSpace(model.Password) &&
                !string.IsNullOrWhiteSpace(model.Role) &&
                (model.Role == "Lecture" || model.Role == "ProgrammeCoordinator" || model.Role == "AcademicManager" || model.Role == "HR"))
            {
                // Create claims for the user - Use fully qualified System.Security.Claims.Claim
                var claims = new List<System.Security.Claims.Claim>
                {
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, model.Username),
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, model.Role) // Assign selected role
                };

                var claimsIdentity = new System.Security.Claims.ClaimsIdentity(claims, "CookieAuth");
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true // Remember me (optional)
                };

                // Sign in the user
                await HttpContext.SignInAsync("CookieAuth", new System.Security.Claims.ClaimsPrincipal(claimsIdentity), authProperties);

                // Redirect to a protected page (e.g., dashboard)
                return RedirectToAction("Index", "Claim"); // Adjusted to match ClaimController
            }

            // Invalid login (e.g., empty fields or bad role)
            ModelState.AddModelError("", "Please enter a username, password, and select a valid role.");
            return View("Index", model);
        }

        // POST: Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Index");
        }

        // Error Action (for global handling)
        [AllowAnonymous]
        public IActionResult Error(int statusCode = 500)
        {
            ViewData["Title"] = $"Error {statusCode}";
            ViewData["StatusCode"] = statusCode;
            ViewData["Message"] = statusCode switch
            {
                404 => "The page you're looking for doesn't exist.",
                403 => "You don't have permission to access this page.",
                500 => "A server error occurred. Please try again or contact support.",
                _ => "An unexpected error happened."
            };
            return View();
        }
    }
}
