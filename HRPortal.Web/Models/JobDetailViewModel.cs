using HRPortal.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRPortal.Web.Models
{
    public class JobDetailViewModel
    {
        public List<TblVacancyAdvert> Vacancies { get; set; }
        public TblVacancyAdvert Vacancy { get; set; }
        public TblSummary Summary { get; set; }
    }
}
