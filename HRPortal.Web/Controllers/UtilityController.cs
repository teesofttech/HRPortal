﻿using HRPortal.Domain.Entities;
using HRPortal.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace HRPortal.Web.Controllers
{
    public class UtilityController : Controller
    {
        RecruitmentPortalDBContext _context;
        public UtilityController(RecruitmentPortalDBContext db_A54634_PortalContext)
        {
            _context = db_A54634_PortalContext;
        }
        [System.Web.Mvc.ValidateInput(false)]
        public IActionResult Index(
           string onlinePresence,
           string projectexperience,
           string skill,
           string education,
           string objective,
           string experiencesummary,
           string vacancyId, string personalinformation)
        {
            Strength strength = new Strength();
            string status = "";
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int vaca = Convert.ToInt32(vacancyId);
                var getcheck = _context.TblApplications.Where(c => c.UserId == userId && c.VacancyId == vaca).FirstOrDefault();
                if (getcheck == null)
                {
                    TblApplication tblApplication = new TblApplication();
                    tblApplication.PersonalInformation = personalinformation;
                    tblApplication.Education = education;
                    tblApplication.Experiencesummary = experiencesummary;
                    tblApplication.Objective = objective;
                    tblApplication.Onlinepresence = onlinePresence;
                    tblApplication.Projectexperience = projectexperience;
                    tblApplication.Skill = skill;
                    tblApplication.UserId = userId;
                    tblApplication.Date = DateTime.UtcNow;
                    tblApplication.VacancyId = Convert.ToInt32(vacancyId);
                    _context.TblApplications.Add(tblApplication);
                    _context.SaveChanges();

                    strength.status = "true";
                    return Json(strength);
                }
                else
                {
                    strength.status = "exist";
                    return Json(strength);
                }
            }
            catch (Exception ex)
            {
                strength.status = "false";
                return Json(strength);
            }
        }
    }
}
