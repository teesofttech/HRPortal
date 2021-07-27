using DocumentParser.src.parser;
using HRPortal.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HRPortal.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        public readonly IHostingEnvironment _hostingEnvironment;
        public DashboardController(IHostingEnvironment hostingEnvironment)
        {

            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index()
        {
            return View();
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
    }
}
