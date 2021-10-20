using HRPortal.Domain.Entities;
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
           string experiencesummary, string vacancyId)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            TblApplication tblApplication = new TblApplication();
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


        }
    }
}
