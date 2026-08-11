using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker.Models.Models.DTOs.Dashboard
{
    public class DashboardProjectDTO
    {
        public int Id { get; set; }

        public string ProjectName { get; set; } = string.Empty;

        public int Progress { get; set; }
    }
}
