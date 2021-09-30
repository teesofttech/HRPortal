using HRPortal.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;

namespace HRPortal.Web.Controllers
{
    [Authorize]
    public class HRDashboardController : Microsoft.AspNetCore.Mvc.Controller
    {
        db_a54634_portalContext db;
        public HRDashboardController(db_a54634_portalContext db)
        {
            this.db = db;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult PostJob()
        {
            return View();
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public async Task<IActionResult> PostJob(TblVacancyAdvert model)
        {
            if (model == null)
            {
                TempData["error"] = "Please fill all the empty(s) and try again in few minutes";
                return View();
            }
            else
            {
                TblVacancyAdvert advert = new TblVacancyAdvert();
                advert.ContractType = model.ContractType;
                advert.Datecreated = DateTime.UtcNow;
                advert.DateOfAdvert = DateTime.UtcNow;
                advert.EndDate = model.EndDate;
                advert.JobCode = model.JobCode;
                advert.JobObjectives = model.JobObjectives;
                advert.JobTitle = model.JobTitle;
                advert.Location = model.Location;
                advert.PostedBy = User.Identity.Name;
                advert.RelevantRequirement = model.RelevantRequirement;
                advert.StartDate = DateTime.UtcNow;
                advert.Status = "Open";
                advert.TravelRequirements = model.TravelRequirements;
                db.TblVacancyAdverts.Add(advert);
                var success = await db.SaveChangesAsync() > 0;
                if (success)
                {
                    TempData["success"] = "Job posted successfully";
                    return View();
                }
                else
                {
                    TempData["error"] = "Opps! We encounter an error while creating the summary. Kindly try again in few minutes";
                    return View();
                }

            }

        }

        public IActionResult Summary()
        {
            return View();
        }

        [ValidateInput(false)]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public async Task<IActionResult> Summary(string companySummary, string equalityStatement)
        {
            TblSummary tblSummary = new TblSummary();
            tblSummary.CompanySummary = companySummary;
            tblSummary.EqualityStatement = equalityStatement;
            db.TblSummaries.Add(tblSummary);
            var success = await db.SaveChangesAsync() > 0;
            if (success)
            {
                TempData["success"] = "Company summary and Equality statement has been saved successfully";
                return View();
            }
            else
            {
                TempData["error"] = "Opps! We encounter an error while creating the summary. Kindly try again in few minutes";
                return View();
            }
        }

        public async Task<IActionResult> ViewSummary()
        {
            return View(await db.TblSummaries.FirstOrDefaultAsync());
        }



    }
}
