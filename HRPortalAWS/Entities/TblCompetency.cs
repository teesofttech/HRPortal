﻿using System;
using System.Collections.Generic;

#nullable disable

namespace HRPortalAWS.Entities
{
    public partial class TblCompetency
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        public virtual AspNetUser User { get; set; }
    }
}
