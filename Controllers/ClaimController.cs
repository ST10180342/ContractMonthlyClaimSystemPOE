
using Microsoft.AspNetCore.Mvc;
using ContractMonthlyClaimSystem.Models;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class ClaimController : Controller
    {
        public IActionResult Submit()
        {
            return View();
        }

        public IActionResult Verify()
        {
            return View();
        }

        public IActionResult Approve()
        {
            return View();
        }

        public IActionResult Track()
        {
            return View();
        }
    }
}