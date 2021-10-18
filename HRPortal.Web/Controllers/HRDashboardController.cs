using HRPortal.Domain.Entities;
using HRPortal.Web.Models;
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
                advert.Department = model.Department;
                //  advert.CompanySummary = model.CompanySummary;
                advert.TravelRequirements = model.TravelRequirements;
                db.TblVacancyAdverts.Add(advert);
                var success = await db.SaveChangesAsync() > 0;
                if (success)
                {
                    TempData["success"] = "Job posted successfully";
                    return RedirectToAction("PostJob", "HRDashboard");
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
                return RedirectToAction("ViewSummary", "HRDashboard");
            }
            else
            {
                TempData["error"] = "Opps! We encounter an error while creating the summary. Kindly try again in few minutes";
                return RedirectToAction("ViewSummary", "HRDashboard");
            }
        }

        public async Task<IActionResult> ViewSummary()
        {
            return View(await db.TblSummaries.FirstOrDefaultAsync());
        }

        public async Task<IActionResult> AllPostedJobs()
        {
            var result = db.TblVacancyAdverts.ToList().OrderByDescending(c => c.Datecreated);//(c => c.Datecreated).ToList();
            return View(result);
        }

        public async Task<IActionResult> GetSingle(int id)
        {
            var result = db.TblVacancyAdverts.Where(c => c.Id == id).FirstOrDefault();

            return View(result);
        }

        public async Task<IActionResult> JobDetail(int id)
        {
            JobDetailViewModel jobDetailViewModel = new JobDetailViewModel();
            var result = db.TblVacancyAdverts.Where(c => c.Id == id).FirstOrDefault();
            jobDetailViewModel.Summary = db.TblSummaries.FirstOrDefault();
            jobDetailViewModel.Vacancy = result;
            return View(jobDetailViewModel);
        }

    }
}
