using HRPortal.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRPortal.Web.Models
{
    public class JobViewModelList
    {
        public int Id { get; set; }
        public string vacancyId { get; set; }
        public string userId { get; set; }
        public string Location { get; set; }
        public string JobTitle { get; set; }
        public string JobCode { get; set; }
        public string SubmittedDate { get; set; }
        public string JobObjectives { get; set; }
        public AspNetUser UserInfo { get; set; }
    }
}
