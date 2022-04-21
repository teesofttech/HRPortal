using HRPortal.Domain.Entities;
using HRPortal.Web.Helper;
using HRPortal.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
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
                    var apiKey = "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";
                    var client = new SendGridClient(apiKey);
                    var from = new EmailAddress("No-Reply@nsiainsurance.com", "NSIA INSURANCE");
                    string fullname = get.FirstName + " " + get.LastName;
                    var to = new EmailAddress(get.Email, fullname);
                    var subject = "Password Reset";
                    string message = "We received your password reset request. Please use the below password to login. <br/> Email: " + email + " <br/> Password: " + password + " <br/> Thank you.";
                    var msg = MailHelper.CreateSingleEmail(from, to, subject, "", message);
                    var responses = await client.SendEmailAsync(msg);
                    //if (responses.IsSuccessStatusCode)
                    //{
                    //    TempData["success"] = "Email sent successfully";
                    //    return RedirectToAction("ViewApplication", "HRDashboard", new { id = userId, id2 = appicationId });
                    //}
                    //else
                    //{
                    //    TempData["error"] = "Error occurred";
                    //    return RedirectToAction("ViewApplication", "HRDashboard", new { id = userId, id2 = appicationId });
                    //}

                    TempData["success"] = "Email sent successfully";
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
