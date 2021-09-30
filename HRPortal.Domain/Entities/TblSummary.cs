using System;
using System.Collections.Generic;

#nullable disable

namespace HRPortal.Domain.Entities
{
    public partial class TblSummary
    {
        public int Id { get; set; }
        public string CompanySummary { get; set; }
        public string EqualityStatement { get; set; }
    }
}
