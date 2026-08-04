
namespace ProjectTracker.Models.Models.DTOs.Project
{
    public class ProjectsDashboardDTO
    {
        public int TotalProjects { get; set; }

        public int ActiveProjects { get; set; }

        public int CompletedProjects { get; set; }

        public int OnHoldProjects { get; set; }

        public int InactiveProjects { get; set; }

        public int TotalTasks { get; set; }

        public int CompletedTasks { get; set; }

        public int InProgressTasks { get; set; }

        public int PendingTasks { get; set; }
        public List<ProjectTracker.Models.Models.Entities.Project> Projects { get; set; } = new();
    }
}
