using System;
using System.Collections.Generic;

#nullable disable

namespace HRPortal.Domain.Entities
{
    public partial class TblManualCvPath
    {
        public int Id { get; set; }
        public string Cvpath { get; set; }
        public string UserId { get; set; }
        public string Pdf { get; set; }
    }
}
