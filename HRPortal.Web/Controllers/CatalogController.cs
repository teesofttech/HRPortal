﻿using HRPortal.Domain.Entities;
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

        public IActionResult JobDetail(int id)
        {
            JobDetailViewModel jobDetailViewModel = new JobDetailViewModel();
            var result = db.TblVacancyAdverts.Where(c => c.Id == id).FirstOrDefault();
            jobDetailViewModel.Summary = db.TblSummaries.FirstOrDefault();
            jobDetailViewModel.Vacancy = result;
            return View(jobDetailViewModel);
        }

        public IActionResult Category(string id)
        {
            JobDetailViewModel jobDetailViewModel = new JobDetailViewModel();
            var result = db.TblVacancyAdverts.Where(c => c.Department == id).ToList();
            jobDetailViewModel.Summary = db.TblSummaries.FirstOrDefault();
            jobDetailViewModel.Vacancies = result;
            return View(jobDetailViewModel);

        }
    }
}
