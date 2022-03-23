using DocumentParser.src.parser;
using GemBox.Document;
using HRPortal.Domain.Entities;
using HRPortal.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xceed.Words.NET;

namespace HRPortal.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        public readonly IHostingEnvironment _hostingEnvironment;
        RecruitmentPortalDBContext db;
        public DashboardController(IHostingEnvironment hostingEnvironment, RecruitmentPortalDBContext db)
        {
            this.db = db;
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            UserDashboardModel userDashboardModel = new UserDashboardModel();
            var getVacanies = await db.TblVacancyAdverts.Where(c => c.Status == "Open").OrderByDescending(c => c.Datecreated).ToListAsync();
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var getUser = await db.AspNetUsers.Where(c => c.Id == userId).FirstOrDefaultAsync();
            if (getUser != null)
            {
                userDashboardModel.AspNetUser = getUser;
                userDashboardModel.Vacanies = getVacanies;
                return View(userDashboardModel);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult MyResume()
        {
            return View();
        }

        public IActionResult CreateResume()
        {
            return View();
        }

        public IActionResult UploadResume()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UploadResume(IFormFile formFile)
        {
            string logoFileName = null;
            string pathfile = null;

            string cacFileName = null;
            string pathfile1 = null;
            try
            {
                if (formFile == null)
                {
                    ViewData["error"] = "No image for either company logo and company document";
                    return RedirectToAction("UploadResume", "Dashboard");
                }
                else
                {
                    string _logo = Path.Combine(_hostingEnvironment.WebRootPath, "cv");
                    logoFileName = Guid.NewGuid().ToString() + "_" + formFile.FileName;
                    pathfile = Path.Combine(_logo, logoFileName);
                    FileStream fs = new FileStream(pathfile, FileMode.Create);
                    formFile.CopyTo(fs);

                    fs.Close();


                    var extension = Path.GetExtension(formFile.FileName);
                    const string mimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    if (string.Equals(extension, ".docx") && formFile.ContentType.Equals(mimeType))
                    {

                        //var appDataPath = Server.MapPath("~/App_Data");
                        //var path = Path.Combine(appDataPath, "Upload", fileName);
                        //file.SaveAs(path);

                        var dictionaryPath = Path.Combine(_hostingEnvironment.WebRootPath, "KeywordDictionary.xml");
                        var modelData = Engine.Parse(pathfile, dictionaryPath);
                        return View(JsonConvert.DeserializeObject<DocumentViewModel>(modelData));
                    }
                    else
                    {
                        //ViewData["success"] = "Your account has been created successfully";
                        return RedirectToAction("UploadResume", "Dashboard");
                    }
                }


            }
            catch (Exception ex)
            {
                ViewData["error"] = "Errors occurred while creating vendor";
                return RedirectToAction("UploadResume", "UploadResume");
            }
        }

        public async Task<IActionResult> ListJobs()
        {
            UserDashboardModel userDashboardModel = new UserDashboardModel();
            var getVacanies = await db.TblVacancyAdverts.Where(c => c.Status == "Open").OrderByDescending(c => c.Datecreated).ToListAsync();
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var getUser = await db.AspNetUsers.Where(c => c.Id == userId).FirstOrDefaultAsync();
            if (getUser != null)
            {
                userDashboardModel.AspNetUser = getUser;
                userDashboardModel.Vacanies = getVacanies;
                return View(userDashboardModel);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> JobDetail(int id)
        {
            UserDashboardModel userDashboardModel = new UserDashboardModel();
            var getVacanies = await db.TblVacancyAdverts.Where(c => c.Id == id).FirstOrDefaultAsync();
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var getUser = await db.AspNetUsers.Where(c => c.Id == userId).FirstOrDefaultAsync();
            if (getUser != null)
            {
                userDashboardModel.AspNetUser = getUser;
                userDashboardModel.TblVacancyAdvert = getVacanies;
                userDashboardModel.TblSummary = await db.TblSummaries.FirstOrDefaultAsync();
                return View(userDashboardModel);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> ViewApplied(int id)
        {
            UserDashboardModel userDashboardModel = new UserDashboardModel();
            var getVacanies = await db.TblVacancyAdverts.Where(c => c.Id == id).FirstOrDefaultAsync();
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var getUser = await db.AspNetUsers.Where(c => c.Id == userId).FirstOrDefaultAsync();
            if (getUser != null)
            {
                userDashboardModel.AspNetUser = getUser;
                userDashboardModel.TblVacancyAdvert = getVacanies;
                userDashboardModel.TblSummary = await db.TblSummaries.FirstOrDefaultAsync();
                return View(userDashboardModel);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public async Task<IActionResult> Apply(int id)
        {
            DocumentViewModel documentViewModel = new DocumentViewModel();
            documentViewModel.vacancyId = id;
            return View(documentViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Apply(IFormFile formFile, int vacancyId)
        {
            string logoFileName = null;
            string pathfile = null;

            string cacFileName = null;
            string pathfile1 = null;
            try
            {
                if (formFile == null)
                {
                    ViewData["error"] = "No image for either company logo and company document";
                    return RedirectToAction("UploadResume", "Dashboard");
                }
                else
                {
                    string _logo = Path.Combine(_hostingEnvironment.WebRootPath, "cv");
                    logoFileName = Guid.NewGuid().ToString() + "_" + formFile.FileName;
                    pathfile = Path.Combine(_logo, logoFileName);
                    FileStream fs = new FileStream(pathfile, FileMode.Create);
                    formFile.CopyTo(fs);

                    fs.Close();

                    var extension = Path.GetExtension(formFile.FileName);
                    const string mimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    if (string.Equals(extension, ".docx") && formFile.ContentType.Equals(mimeType))
                    {
                        var dictionaryPath = Path.Combine(_hostingEnvironment.WebRootPath, "KeywordDictionary.xml");
                        var modelData = Engine.Parse(pathfile, dictionaryPath);
                        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;



                        DocX document = null;

                        document = DocX.Load(pathfile);
                        MemoryStream ms = new MemoryStream();
                        document.SaveAs(ms);
                        byte[] byteArray = ms.ToArray();

                        ComponentInfo.SetLicense("DN-2020Dec21-1ssglziNs2d2QcHZXrjIL6TFUebyheESFOwULzxjITFdIVG1A86Q7YJoRsIqH1UgT4N4xgXgUjG934hcq8luzGNl/gg==A");

                        // In order to convert Word to PDF, we just need to:
                        // 1. Load DOC or DOCX file into DocumentModel object.
                        // 2. Save DocumentModel object to PDF file.
                        DocumentModel documentt = DocumentModel.Load(ms);

                        string _logo1 = Path.Combine(_hostingEnvironment.WebRootPath, "cv_pdf");
                        string logoFileName1 = Guid.NewGuid().ToString() + "_" + formFile.FileName;
                        pathfile1 = Path.Combine(_logo1, logoFileName1);

                        documentt.Save(pathfile1 + ".pdf");
                        TblCvPath tblCvPath = new TblCvPath();
                        tblCvPath.Cvpath = logoFileName;
                        tblCvPath.UserId = userId;
                        tblCvPath.VacancyId = vacancyId;
                        tblCvPath.Pdf = logoFileName1 + ".pdf";
                        db.TblCvPaths.Add(tblCvPath);
                        var success = await db.SaveChangesAsync() > 0;

                        TempData["vacancyId"] = vacancyId;
                        return View(JsonConvert.DeserializeObject<DocumentViewModel>(modelData));
                    }
                    else
                    {
                        //ViewData["success"] = "Your account has been created successfully";
                        return RedirectToAction("UploadResume", "Dashboard");
                    }
                }
            }
            catch (Exception ex)
            {
                ViewData["error"] = "Errors occurred while creating vendor";
                return RedirectToAction("UploadResume", "UploadResume");
            }
        }

        public async Task<IActionResult> Applied()
        {
            //@model HRPortal.Web.Models.UserDashboardModel
            UserDashboardModel2 userDashboardModel = new UserDashboardModel2();

            List<JobViewModelList> jobDetailViewModels = new List<JobViewModelList>();
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var get = await db.TblApplications.Where(c => c.UserId == userId).ToListAsync();
            foreach (var item in get)
            {
                var getJob = await db.TblVacancyAdverts.Where(c => c.Id == item.VacancyId).FirstOrDefaultAsync();

                JobViewModelList jobDetailViewModel = new JobViewModelList();
                jobDetailViewModel.Id = item.Id;
                jobDetailViewModel.JobObjectives = getJob.JobObjectives;
                jobDetailViewModel.userId = item.UserId;
                jobDetailViewModel.vacancyId = Convert.ToString(item.VacancyId);
                jobDetailViewModel.JobCode = getJob.JobCode;
                jobDetailViewModel.JobTitle = getJob.JobTitle;
                jobDetailViewModel.Location = getJob.Location;
                jobDetailViewModel.SubmittedDate = item.Date.Value.ToLongDateString();
                jobDetailViewModels.Add(jobDetailViewModel);
            }
            var getUser = await db.AspNetUsers.Where(c => c.Id == userId).FirstOrDefaultAsync();
            if (getUser != null)
            {
                userDashboardModel.AspNetUser = getUser;

            }

            userDashboardModel.jobDetailViewModels = jobDetailViewModels;
            return View(userDashboardModel);
        }

        public async Task<IActionResult> Question(int id)
        {
            var get = db.TblQuestions.Where(c => c.VacanyId == id).FirstOrDefault();
            return View(get);
        }

        public async Task<IActionResult> CreateCV()
        {
            return View();
        }

        [HttpPost]
        [System.Web.Mvc.ValidateInput(false)]
        public async Task<IActionResult> CreateCV(string Objective, string Onlinepresence, string Projectexperience,
            string Skill, string Education, string Experiencesummary, string PersonalInformation)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            TblResume resume = new TblResume()
            {
                Date = DateTime.UtcNow,
                Education = Education,
                Experiencesummary = Experiencesummary,
                PersonalInformation = PersonalInformation,
                Objective = Objective,
                Onlinepresence = Onlinepresence,
                Projectexperience = Projectexperience,
                Skill = Skill,
                UserId = userId
            };
            db.TblResumes.Add(resume);
            db.SaveChanges();
            TempData["success"] = "Resume created successfully";
            return View();
        }

    }
}
