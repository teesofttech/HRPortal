using System;
using System.Collections.Generic;

#nullable disable

namespace HRPortalAWS.Entities
{
    public partial class TblVacancyAdvert
    {
        public int Id { get; set; }
        public DateTime? DateOfAdvert { get; set; }
        public string JobTitle { get; set; }
        public string JobCode { get; set; }
        public string Location { get; set; }
        public string ContractType { get; set; }
        public string TravelRequirements { get; set; }
        public string JobObjectives { get; set; }
        public string RelevantRequirement { get; set; }
        public string Status { get; set; }
        public DateTime? Datecreated { get; set; }
        public string PostedBy { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Department { get; set; }
    }
}
