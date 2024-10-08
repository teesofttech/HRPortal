﻿using DocumentParser.src.parser;
using GemBox.Document;
using HRPortal.Domain.Entities;
using HRPortal.Web.Models;
using HtmlAgilityPack;
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
using System.Web;
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
            try
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
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public async Task<IActionResult> JobDetail(int id)
        {
            try
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
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public async Task<IActionResult> ViewApplied(int id)
        {
            try
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
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }
        public async Task<IActionResult> Apply(int id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var check = db.TblResumes.Where(c => c.UserId == userId).FirstOrDefault();

                DocumentViewModel documentViewModel = new DocumentViewModel();
                documentViewModel.vacancyId = id;
                documentViewModel.Resume = check;
                return View(documentViewModel);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Dashboard");
            }
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

        public async Task<IActionResult> UseGeneratedCV(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var getResume = db.TblResumes.Where(c => c.UserId == userId).FirstOrDefault();

            int vaca = Convert.ToInt32(id);
            var getcheck = db.TblApplications.Where(c => c.UserId == userId && c.VacancyId == vaca).FirstOrDefault();
            if (getcheck == null)
            {
                TblApplication tblApplication = new TblApplication();
                tblApplication.PersonalInformation = getResume.PersonalInformation;
                tblApplication.Education = getResume.Education;
                tblApplication.Experiencesummary = getResume.Experiencesummary;
                tblApplication.Objective = getResume.Objective;
                tblApplication.Onlinepresence = getResume.Onlinepresence;
                tblApplication.Projectexperience = getResume.Projectexperience;
                tblApplication.Skill = getResume.Skill;
                tblApplication.UserId = userId;
                tblApplication.Date = DateTime.UtcNow;
                tblApplication.VacancyId = Convert.ToInt32(id);
                db.TblApplications.Add(tblApplication);
                db.SaveChanges();

                TempData["success"] = "Application submitted successfully";
                return RedirectToAction("Completed", "Dashboard");
            }
            else
            {
                TempData["error"] = "Error occurred while submitting the Application";
                return RedirectToAction("Error", "Dashboard");
            }
        }

        public async Task<IActionResult> Completed()
        {
            UserDashboardModel userDashboardModel = new UserDashboardModel();
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var getUser = await db.AspNetUsers.Where(c => c.Id == userId).FirstOrDefaultAsync();
            if (getUser != null)
            {
                userDashboardModel.AspNetUser = getUser;
                return View(userDashboardModel);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> Error()
        {
            UserDashboardModel userDashboardModel = new UserDashboardModel();
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var getUser = await db.AspNetUsers.Where(c => c.Id == userId).FirstOrDefaultAsync();
            if (getUser != null)
            {
                userDashboardModel.AspNetUser = getUser;
                return View(userDashboardModel);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }
        public async Task<IActionResult> Applied()
        {
            //@model HRPortal.Web.Models.UserDashboardModel
            try
            {
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
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }


        public async Task<IActionResult> Question(int id)
        {
            try
            {
                var get = db.TblQuestions.Where(c => c.VacanyId == id).FirstOrDefault();
                return View(get);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public async Task<IActionResult> CreateCV()
        {
            try
            {
                UserDashboardModel userDashboardModel = new UserDashboardModel();
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var getUser = await db.AspNetUsers.Where(c => c.Id == userId).FirstOrDefaultAsync();
                if (getUser != null)
                {
                    userDashboardModel.AspNetUser = getUser;
                    return View(userDashboardModel);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }

        [HttpPost]
        [System.Web.Mvc.ValidateInput(false)]
        public async Task<IActionResult> CreateCV(string Objective, string Online, string Projects,
            string Skills, string Education, string ExecutiveSummary, string personal, string Certification)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                //check if resume is avail for the user 
                var userResume = await db.TblResumes.Where(c => c.UserId == userId).FirstOrDefaultAsync();
                if (userResume != null)
                {
                    TempData["exist"] = "You have added a resume before!!!";
                    return RedirectToAction("CreateCV", "Dashboard");
                }
                else
                {
                    TblResume resume = new TblResume()
                    {
                        Date = DateTime.UtcNow,
                        Education = Education,
                        Experiencesummary = ExecutiveSummary,
                        PersonalInformation = personal,
                        Objective = Objective,
                        Onlinepresence = Online,
                        Projectexperience = Projects,
                        Skill = Skills,
                        UserId = userId,
                        Certification = Certification
                    };
                    db.TblResumes.Add(resume);
                    db.SaveChanges();

                    //create the cv on the fly 

                    ComponentInfo.SetLicense("DN-2020Dec21-1ssglziNs2d2QcHZXrjIL6TFUebyheESFOwULzxjITFdIVG1A86Q7YJoRsIqH1UgT4N4xgXgUjG934hcq8luzGNl/gg==A");

                    //string path = Path.Combine(this.Environment.WebRootPath, "cv/") + applicationVM.TblCvPath.Cvpath;
                    //convert the docx to pdf on the fly
                    var path = Path.Combine(this._hostingEnvironment.WebRootPath, "cv_pdf/temp.docx");
                    var document = DocumentModel.Load(path);

                    // Execute find and replace operations.


                    HtmlDocument htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(personal);
                    string pp = htmlDoc.DocumentNode.InnerText;

                    HtmlDocument htmlSummary = new HtmlDocument();
                    htmlSummary.LoadHtml(ExecutiveSummary);
                    string summary = htmlSummary.DocumentNode.InnerText;

                    HtmlDocument SkillsSummary = new HtmlDocument();
                    SkillsSummary.LoadHtml(Skills);
                    string _skills = SkillsSummary.DocumentNode.InnerText;

                    HtmlDocument experienceContent = new HtmlDocument();
                    experienceContent.LoadHtml(Projects);
                    string _expereicen = experienceContent.DocumentNode.InnerText;

                    HtmlDocument certificationContent = new HtmlDocument();
                    certificationContent.LoadHtml(Certification);
                    string _certification = certificationContent.DocumentNode.InnerText;

                    HtmlDocument educationContent = new HtmlDocument();
                    educationContent.LoadHtml(Education);
                    string _education = educationContent.DocumentNode.InnerText;

                    HtmlDocument onlineContent = new HtmlDocument();
                    onlineContent.LoadHtml(Online);
                    string _online = onlineContent.DocumentNode.InnerText;


                    document.Content.Replace("{{PersonalInformation}}", pp);
                    document.Content.Replace("{{Summary}}", summary);
                    document.Content.Replace("{{Skills}}", _skills);
                    document.Content.Replace("{{Experience}}", _expereicen);
                    document.Content.Replace("{{Certification}}", _certification);
                    document.Content.Replace("{{Education}}", _education);
                    document.Content.Replace("{{Online}}", _online);

                    //   Html.Raw(HttpUtility.HtmlDecode(ViewData["HTMLData"].ToString()));

                    // Save document in specified file format.
                    using var stream = new MemoryStream();
                    string savepath = Path.Combine(this._hostingEnvironment.WebRootPath, "cv_pdf/") + userId + ".pdf";
                    string savepathWord = Path.Combine(this._hostingEnvironment.WebRootPath, "cv_pdf/") + userId + ".docx";
                    document.Save(savepath, GemBox.Document.SaveOptions.PdfDefault);
                    document.Save(savepathWord, GemBox.Document.SaveOptions.DocxDefault);

                    TblManualCvPath cvPath = new TblManualCvPath();
                    cvPath.UserId = userId;
                    cvPath.Pdf = userId + ".pdf";
                    cvPath.Cvpath = userId + ".docx";
                    db.TblManualCvPaths.Add(cvPath);
                    db.SaveChanges();

                    TempData["success"] = "Resume created successfully";
                    return RedirectToAction("CreateCV", "Dashboard");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public async Task<IActionResult> ViewCV()
        {
            try
            {
                UserDashboardModel userDashboardModel = new UserDashboardModel();
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userResume = await db.TblResumes.Where(c => c.UserId == userId).FirstOrDefaultAsync();
                var getUser = await db.AspNetUsers.Where(c => c.Id == userId).FirstOrDefaultAsync();
                if (getUser != null && userResume == null)
                {
                    userDashboardModel.AspNetUser = getUser;
                    TempData["exist"] = "You are yet to create a resume on our platform!!!";
                    return RedirectToAction("CreateCV", "Dashboard");
                }
                else
                {
                    var manualCv = await db.TblManualCvPaths.Where(c => c.UserId == userId).FirstOrDefaultAsync();
                    userDashboardModel.AspNetUser = getUser;
                    userDashboardModel.Manual = manualCv;
                    userDashboardModel.Resume = userResume;
                    return View(userDashboardModel);
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public async Task<IActionResult> DownloadCv(string id)
        {
            try
            {
                var manualCv = await db.TblManualCvPaths.Where(c => c.UserId == id).FirstOrDefaultAsync();
                ComponentInfo.SetLicense("DN-2020Dec21-1ssglziNs2d2QcHZXrjIL6TFUebyheESFOwULzxjITFdIVG1A86Q7YJoRsIqH1UgT4N4xgXgUjG934hcq8luzGNl/gg==A");
                var getUser = await db.AspNetUsers.Where(c => c.Id == id).FirstOrDefaultAsync();
                //string path = Path.Combine(this.Environment.WebRootPath, "cv/") + applicationVM.TblCvPath.Cvpath;
                //convert the docx to pdf on the fly
                var path = Path.Combine(this._hostingEnvironment.WebRootPath, "cv_pdf/") + manualCv.Cvpath;
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
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public async Task<IActionResult> ChangePassword()
        {
            try
            {
                UserDashboardModel userDashboardModel = new UserDashboardModel();
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var getUser = await db.AspNetUsers.Where(c => c.Id == userId).FirstOrDefaultAsync();
                if (getUser != null)
                {
                    userDashboardModel.AspNetUser = getUser;
                    return View(userDashboardModel);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> ChangePassword(string newPassword)
        //{
        //    UserDashboardModel userDashboardModel = new UserDashboardModel();
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //    return View();
        //}
    }
}
