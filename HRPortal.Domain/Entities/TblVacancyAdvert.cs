using System;
using System.Collections.Generic;

#nullable disable

namespace HRPortal.Domain.Entities
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
        public string ReportsTo { get; set; }
        public string DirectReports { get; set; }
        public string CoreResponisbilities { get; set; }
        public string Knowledge { get; set; }
        public string Education { get; set; }
        public string Skills { get; set; }
        public string Experience { get; set; }
        public string ProfQualitification { get; set; }
        public string BehaviouralCompetencies { get; set; }
    }
}
