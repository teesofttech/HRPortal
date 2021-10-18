using System;
using System.Collections.Generic;

#nullable disable

namespace HRPortalAWS.Entities
{
    public partial class TblCandidateApplication
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Phonenumber { get; set; }
        public string Nationality { get; set; }
        public string CountryOfResidence { get; set; }
        public string StateOfResidence { get; set; }
        public string WorkExperience { get; set; }
        public string ProfessionalQualification { get; set; }
        public string PermanentStaffNsia { get; set; }
        public string CompletedNysc { get; set; }
        public string UserId { get; set; }
        public string Availability { get; set; }
        public DateTime Date { get; set; }

        public virtual AspNetUser User { get; set; }
    }
}
