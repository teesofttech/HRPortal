using ClosedXML.Excel;
using GemBox.Document;
using HRPortal.Domain.Entities;
using HRPortal.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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

        private IHostingEnvironment Environment;



        public HRDashboardController(RecruitmentPortalDBContext db, IHostingEnvironment _environment)
        {
            this.db = db; Environment = _environment;
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

                advert.BehaviouralCompetencies = model.BehaviouralCompetencies;
                advert.CoreResponisbilities = model.CoreResponisbilities;
                advert.DirectReports = model.DirectReports;
                advert.Education = model.Education;
                advert.Experience = model.Experience;
                advert.Knowledge = model.Knowledge;
                advert.ProfQualitification = model.ProfQualitification;
                advert.ReportsTo = model.ReportsTo;
                advert.Skills = model.Skills;

                db.TblVacancyAdverts.Add(advert);
                var success = await db.SaveChangesAsync() > 0;
                if (success)
                {
                    TempData["success"] = "Job posted successfully";
                    return RedirectToAction("Question", "HRDashboard", new { id = advert.Id });
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

        public async Task<IActionResult> List()
        {
            var result = db.TblVacancyAdverts.ToList().OrderByDescending(c => c.Datecreated);//(c => c.Datecreated).ToList();
            return View(result);
        }

        public async Task<IActionResult> ChangeStatus(int id)
        {
            var get = db.TblVacancyAdverts.Where(c => c.Id == id).FirstOrDefault();
            get.Status = "Closed";
            db.SaveChanges();
            TempData["success"] = "Status updated";
            return RedirectToAction("List", "HRDashboard");
        }


        public async Task<IActionResult> ChangeOpenStatus(int id)
        {
            var get = db.TblVacancyAdverts.Where(c => c.Id == id).FirstOrDefault();
            get.Status = "Open";
            db.SaveChanges();
            TempData["success"] = "Status updated";
            return RedirectToAction("List", "HRDashboard");
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

        public async Task<IActionResult> DownloadApplicantsRecord(int id)
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
                jobDetailViewModel.FirstName = getUser.FirstName;
                jobDetailViewModel.LastName = getUser.LastName;
                jobDetailViewModel.Email = getUser.Email;
                jobDetailViewModel.Title = getJob.JobTitle;
                jobDetailViewModel.DateofAdvert = getJob.DateOfAdvert.Value.ToLongDateString();
                jobDetailViewModels.Add(jobDetailViewModel);
            }

            DataTable dt = new DataTable("Applicants");
            dt.Columns.AddRange(new DataColumn[7] {
                                            new DataColumn("DateOfAdvert"),
                                            new DataColumn("FirstName"),
                                            new DataColumn("LastName"),
                                            new DataColumn("JobCode"),
                                            new DataColumn("JobTitle"),
                                            new DataColumn("Location"),
                                            new DataColumn("SubmittedDate")
                                           });
            foreach (var customer in jobDetailViewModels)
            {
                dt.Rows.Add(
                    customer.DateofAdvert,
                    customer.FirstName,
                    customer.LastName,
                    customer.JobCode,
                    customer.JobTitle,
                    customer.Location,
                    customer.SubmittedDate
                    );
            }
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "GridSMS.xlsx");
                }
            }
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

        [ValidateInput(false)]
        public async Task<IActionResult> Notification(string message, string userId, string appicationId)
        {
            var getUser = await db.AspNetUsers.Where(c => c.Id == userId).FirstOrDefaultAsync();
            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("No-Reply@nsiainsurance.com", "NSIA INSURANCE");
            string fullname = getUser.FirstName + " " + getUser.LastName;
            var to = new EmailAddress(getUser.Email, fullname);
            var subject = "Congratulations";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, message, message);
            var responses = await client.SendEmailAsync(msg);
            if (responses.IsSuccessStatusCode)
            {
                TempData["success"] = "Email sent successfully";
                return RedirectToAction("ViewApplication", "HRDashboard", new { id = userId, id2 = appicationId });
            }
            else
            {
                TempData["error"] = "Error occurred";
                return RedirectToAction("ViewApplication", "HRDashboard", new { id = userId, id2 = appicationId });
            }
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public async Task<IActionResult> DownloadCV(int VacancyId, string UserId)
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

                ComponentInfo.SetLicense("DN-2020Dec21-1ssglziNs2d2QcHZXrjIL6TFUebyheESFOwULzxjITFdIVG1A86Q7YJoRsIqH1UgT4N4xgXgUjG934hcq8luzGNl/gg==A");

                //string path = Path.Combine(this.Environment.WebRootPath, "cv/") + applicationVM.TblCvPath.Cvpath;
                //convert the docx to pdf on the fly
                var path = Path.Combine(this.Environment.WebRootPath, "cv/") + applicationVM.TblCvPath.Cvpath;
                var document = DocumentModel.Load(path);

                // Execute find and replace operations.
                //document.Content.Replace("{{Number}}", this.Invoice.Number.ToString("0000"));
                //document.Content.Replace("{{Date}}", this.Invoice.Date.ToString("d MMM yyyy HH:mm"));
                //document.Content.Replace("{{Company}}", this.Invoice.Company);
                //document.Content.Replace("{{Address}}", this.Invoice.Address);
                //document.Content.Replace("{{Name}}", this.Invoice.Name);

                // Save document in specified file format.
                using var stream = new MemoryStream();
                document.Save(stream, GemBox.Document.SaveOptions.PdfDefault);

                // Download file.
                return File(stream.ToArray(), "application/octet-stream", getUser.FirstName + "_CV.pdf");

                //Read the File data into Byte Array.
                //byte[] bytes = System.IO.File.ReadAllBytes(path);

                ////Send the File to Download.
                //return File(bytes, "application/octet-stream", getUser.FirstName + "_CV");
            }

        }

        public async Task<IActionResult> EmailNotification()
        {
            return View();
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public async Task<IActionResult> EmailNotification(string emailaddresses, string message, string subject)
        {
            string[] split = emailaddresses.Split(',');
            foreach (var item in split)
            {
                var apiKey = "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";
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress("No-Reply@nsiainsurance.com", "NSIA INSURANCE");
                //string fullname = getUser.FirstName + " " + getUser.LastName;
                var to = new EmailAddress(item, item);
                //var subject = "Congratulations";

                var msg = MailHelper.CreateSingleEmail(from, to, subject, message, message);
                var responses = await client.SendEmailAsync(msg);
                if (responses.IsSuccessStatusCode)
                {
                    TempData["success"] = "Email sent successfully";

                }
                else
                {
                    TempData["error"] = "Error occurred";

                }
            }
            return RedirectToAction("EmailNotification", "HRDashboard");
        }

        public async Task<IActionResult> Question(int id)
        {
            var get = db.TblVacancyAdverts.Where(c => c.Id == id).FirstOrDefault();
            return View(get);
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public async Task<IActionResult> Question(int vacanyId, string Question1, string Question2, string Question3, string Question4, string Question5)
        {
            TblQuestion tbl = new TblQuestion();
            tbl.VacanyId = vacanyId;
            tbl.Question1 = Question1;
            tbl.Question2 = Question2;
            tbl.Question3 = Question3;
            tbl.Question4 = Question4;
            tbl.Question5 = Question5;
            db.TblQuestions.Add(tbl);
            db.SaveChanges();
            TempData["success"] = "Question has been added";
            return RedirectToAction("ViewQuestions");
        }
        public async Task<IActionResult> CreateQuestion()
        {
            VacancyModel vacancyModel = new VacancyModel();
            var getAllVacancy = db.TblVacancyAdverts.ToList();
            vacancyModel.Adverts = getAllVacancy;
            return View(vacancyModel);
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public async Task<IActionResult> CreateQuestion(VacancyModel model, string Question1, string Question2, string Question3, string Question4, string Question5)
        {
            TblQuestion tbl = new TblQuestion();
            tbl.VacanyId = model.VacancyId;
            tbl.Question1 = Question1;
            tbl.Question2 = Question2;
            tbl.Question3 = Question3;
            tbl.Question4 = Question4;
            tbl.Question5 = Question5;
            db.TblQuestions.Add(tbl);
            db.SaveChanges();
            TempData["success"] = "Question has been added";
            return RedirectToAction("ViewQuestions");
        }
        public async Task<IActionResult> ViewQuestions()
        {
            List<QuestionViewModel> questionViewModels = new List<QuestionViewModel>();
            var get = db.TblQuestions.ToList();

            foreach (var item in get)
            {
                var ggg = db.TblVacancyAdverts.Where(c => c.Id == item.VacanyId).FirstOrDefault();
                QuestionViewModel questionView = new QuestionViewModel();
                questionView.JobTitle = ggg.JobTitle;
                questionView.Id = item.Id;
                questionView.Question1 = item.Question1;
                questionView.Question2 = item.Question2;
                questionView.Question3 = item.Question3;
                questionView.Question4 = item.Question4;
                questionView.Question5 = item.Question5;
                questionViewModels.Add(questionView);
            }
            return View(questionViewModels);
        }

        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var get = db.TblQuestions.Where(c => c.Id == id).FirstOrDefault();
            if (get != null)
            {
                db.TblQuestions.Remove(get);
                db.SaveChanges();
                return RedirectToAction("ViewQuestions");
            }
            else
            {
                return RedirectToAction("ViewQuestions");
            }
        }
    }
}
