﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using HRPortal.Web.Data;
using HRPortal.Domain.Entities;
using HRPortal.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using HRPortal.Web.Helper;

namespace HRPortal.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly RecruitmentPortalDBContext _context;
        private readonly ReCaptcha _captcha;
        public LoginModel(SignInManager<ApplicationUser> signInManager,
            ILogger<LoginModel> logger,
            UserManager<ApplicationUser> userManager, RecruitmentPortalDBContext context, ReCaptcha captcha)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
            _captcha = captcha;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {

                if (!Request.Form.ContainsKey("g-recaptcha-response"))
                {
                    TempData["error"] = "Captcha not selected";
                    ModelState.AddModelError(string.Empty, "Captcha not selected");
                    return Page();
                }
                var captcha = Request.Form["g-recaptcha-response"].ToString();

                if (!await _captcha.IsValid(captcha))
                {
                    TempData["error"] = "Captcha not selected";
                    ModelState.AddModelError(string.Empty, "Captcha not selected");
                    return Page();
                }

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");

                    var getUserRole = await _context.AspNetUsers.Where(c => c.Email == Input.Email).FirstOrDefaultAsync();


                    if (getUserRole.Role.Equals("Admin"))
                    {
                        HttpContext.Session.SetString("UserId", getUserRole.Id);
                        HttpContext.Session.SetString("FirstName", getUserRole.FirstName);
                        HttpContext.Session.SetString("LastName", getUserRole.LastName);
                        HttpContext.Session.SetString("Email", getUserRole.Email);
                        HttpContext.Session.SetString("Role", getUserRole.Role);
                        return RedirectToAction("Index", "HRDashboard");
                    }
                    else if (getUserRole.Role.Equals("User"))
                    {
                        return RedirectToAction("Index", "SupportDashboard");
                    }
                    else if (getUserRole.Role.Equals("Candidate"))
                    {
                        return RedirectToAction("Index", "Dashboard");
                    }
                }

                else
                {
                    TempData["error"] = "Invalid Credentials";
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
