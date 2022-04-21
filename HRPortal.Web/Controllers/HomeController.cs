using HRPortal.Domain.Entities;
using HRPortal.Web.Helper;
using HRPortal.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace HRPortal.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private RecruitmentPortalDBContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public HomeController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<HomeController> logger, RecruitmentPortalDBContext db)
        {
            _logger = logger;
            this.db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View(db.TblVacancyAdverts.Where(c => c.Status == "Open").OrderByDescending(c => c.Datecreated).Take(10).ToList());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            string password = RandomGenerator.GetCharNumber(6);
            var get = await _userManager.FindByEmailAsync(email);
            if (get == null)
            {
                TempData["error"] = "Email not found";
                return View();
            }
            else
            {
                string resetToken = await _userManager.GeneratePasswordResetTokenAsync(get);
                IdentityResult passwordChangeResult = await _userManager.ResetPasswordAsync(get, resetToken, password);
                if (passwordChangeResult.Succeeded)
                {
                    TempData["success"] = "Successful";
                    return View();
                }
                else
                {
                    TempData["error"] = "Your password reset cannot be completed at this moment";
                    return View();
                }
            }
        }
    }
}
