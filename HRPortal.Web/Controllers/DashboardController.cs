using DocumentParser.src.parser;
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
            var getVacanies = await db.TblVacancyAdverts.OrderByDescending(c => c.Datecreated).ToListAsync();
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
            var getVacanies = await db.TblVacancyAdverts.OrderByDescending(c => c.Datecreated).ToListAsync();
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

        public async Task<IActionResult> Apply(int id)
        {
            DocumentViewModel documentViewModel = new DocumentViewModel();
            documentViewModel.vacancyId = id;
            return View(documentViewModel);
        }


        [HttpPost]
        public IActionResult Apply(IFormFile formFile, int vacancyId)
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
            return View(jobDetailViewModels);
        }

    }
}
