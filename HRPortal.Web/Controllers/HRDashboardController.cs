using HRPortal.Domain.Entities;
using HRPortal.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;

namespace HRPortal.Web.Controllers
{
    [Authorize]
    public class HRDashboardController : Microsoft.AspNetCore.Mvc.Controller
    {
        RecruitmentPortalDBContext db;
        public HRDashboardController(RecruitmentPortalDBContext db)
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

        public async Task<IActionResult> Applied()
        {
            var result = db.TblVacancyAdverts.ToList().OrderByDescending(c => c.Datecreated);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var getUser = db.AspNetUsers.Where(c => c.Id == userId).FirstOrDefault();
            AppliedVM appliedVM = new AppliedVM();
            appliedVM.VacancyAdvert = result.ToList();
            appliedVM.User = getUser;
            return View(appliedVM);
        }

        public async Task<IActionResult> ViewApplicants(int id)
        {
            List<JobViewModelList> jobDetailViewModels = new List<JobViewModelList>();
            var get = await db.TblApplications.Where(c => c.VacancyId == id).ToListAsync();
            foreach (var item in get)
            {
                var getJob = await db.TblVacancyAdverts.Where(c => c.Id == item.VacancyId).FirstOrDefaultAsync();
                var getUser = await db.AspNetUsers.Where(c => c.Id == item.UserId).FirstOrDefaultAsync();

                JobViewModelList jobDetailViewModel = new JobViewModelList();
                jobDetailViewModel.Id = item.Id;
                jobDetailViewModel.JobObjectives = getJob.JobObjectives;
                jobDetailViewModel.userId = item.UserId;
                jobDetailViewModel.vacancyId = Convert.ToString(item.VacancyId);
                jobDetailViewModel.JobCode = getJob.JobCode;
                jobDetailViewModel.JobTitle = getJob.JobTitle;
                jobDetailViewModel.Location = getJob.Location;
                jobDetailViewModel.SubmittedDate = item.Date.Value.ToLongDateString();
                jobDetailViewModel.UserInfo = getUser;
                jobDetailViewModels.Add(jobDetailViewModel);
            }
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var getUserInfo = db.AspNetUsers.Where(c => c.Id == userId).FirstOrDefault();

            Applied2VM applied2VM = new Applied2VM();
            applied2VM.jobViewModelLists = jobDetailViewModels;
            applied2VM.User = getUserInfo;

            return View(applied2VM);
        }

        public async Task<IActionResult> ViewApplication(string id, int id2)
        {
            var getApplication = db.TblApplications.Where(c => c.UserId == id && c.VacancyId == id2).FirstOrDefault();
            var getUser = await db.AspNetUsers.Where(c => c.Id == id).FirstOrDefaultAsync();
            ApplicationVM applicationVM = new ApplicationVM();
            applicationVM.Application = getApplication;
            applicationVM.User = getUser;
            return View(applicationVM);
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public async Task<IActionResult> ViewCV(int VacancyId, string UserId)
        {
            ApplicationVM2 applicationVM = new ApplicationVM2();
            var getUser = await db.AspNetUsers.Where(c => c.Id == UserId).FirstOrDefaultAsync();
            var get = await db.TblCvPaths.Where(c => c.VacancyId == VacancyId && c.UserId == UserId).FirstOrDefaultAsync();
            if (get == null)
            {
                return View();
            }
            else
            {
                applicationVM.TblCvPath = get;
                applicationVM.User = getUser;
                return View(applicationVM);
            }
        }
    }
}
