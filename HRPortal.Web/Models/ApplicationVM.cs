using HRPortal.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRPortal.Web.Models
{
    public class ApplicationVM
    {
        public AspNetUser User { get; set; }
        public TblApplication Application { get; set; }
    }

    public class AppliedVM
    {
        public AspNetUser User { get; set; }
        public List<TblVacancyAdvert> VacancyAdvert { get; set; }
    }

    public class Applied2VM
    {
        public AspNetUser User { get; set; }
        public List<JobViewModelList> jobViewModelLists { get; set; }
    }
}
