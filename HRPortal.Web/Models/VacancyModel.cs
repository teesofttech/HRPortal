using HRPortal.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRPortal.Web.Models
{
    public class VacancyModel
    {
        public int VacancyId { get; set; }
        public List<TblVacancyAdvert> Adverts { get; set; }
    }

}
