using System;
using System.Collections.Generic;

#nullable disable

namespace HRPortal.Domain.Entities
{
    public partial class TblQuestion
    {
        public int Id { get; set; }
        public int? VacanyId { get; set; }
        public string Question1 { get; set; }
        public string Question3 { get; set; }
        public string Question4 { get; set; }
        public string Question5 { get; set; }
        public string Question2 { get; set; }
        public DateTime? Datetime { get; set; }
    }
}
