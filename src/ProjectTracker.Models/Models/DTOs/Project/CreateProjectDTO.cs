using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker.Models.Models.DTOs.Project
{
    public class CreateProjectDTO
    {
        public string ProjectName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
       
        public string Status { get; set; }
    }
}
