using HRPortal.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRPortal.Web.Models
{
    public class UserDashboardModel
    {
        public AspNetUser AspNetUser { get; set; }
        public List<TblVacancyAdvert> Vacanies { get; set; }
        public TblVacancyAdvert TblVacancyAdvert { get; set; }
        public TblSummary TblSummary { get; set; }
    }

    public class UserDashboardModel2
    {
        public AspNetUser AspNetUser { get; set; }
        public List<JobViewModelList> jobDetailViewModels { get; set; }
    }
}
