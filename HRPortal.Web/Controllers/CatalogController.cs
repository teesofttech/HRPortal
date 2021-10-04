using HRPortal.Domain.Entities;
using HRPortal.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRPortal.Web.Controllers
{
    public class CatalogController : Controller
    {
        db_a54634_portalContext db;
        public CatalogController(db_a54634_portalContext db)
        {
            this.db = db;
        }
        public IActionResult Index()
        {
            JobDetailViewModel jobDetailViewModel = new JobDetailViewModel();
            var result = db.TblVacancyAdverts.OrderByDescending(c => c.Datecreated).ToList();
            jobDetailViewModel.Summary = db.TblSummaries.FirstOrDefault();
            jobDetailViewModel.Vacancies = result;
            return View(jobDetailViewModel);
        }
    }
}
