using System;
using System.Collections.Generic;

#nullable disable

namespace HRPortal.Domain.Entities
{
    public partial class TblApplication
    {
        public int Id { get; set; }
        public int? VacancyId { get; set; }
        public string UserId { get; set; }
        public DateTime? Date { get; set; }
        public string Objective { get; set; }
        public string Onlinepresence { get; set; }
        public string Projectexperience { get; set; }
        public string Skill { get; set; }
        public string Education { get; set; }
        public string Experiencesummary { get; set; }
    }
}
