using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRPortal.Web.Models
{
    public class QuestionViewModel
    {
        public int Id { get; set; }
        public string JobTitle { get; set; }
        public string Question1 { get; set; }
        public string Question2 { get; set; }
        public string Question3 { get; set; }
        public string Question4 { get; set; }
        public string Question5 { get; set; }
    }
}
