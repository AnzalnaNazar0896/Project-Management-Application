using ProjectTracker.Models.Models.DTOs.Notification;
using ProjectTracker.Models.Models.DTOs.Project;
using ProjectTracker.Models.Models.DTOs.Sprint;
using ProjectTracker.Models.Models.DTOs.Task;

namespace ProjectTracker.Models.Models.DTOs.Dashboard
{
    public class DashboardIndexDTO
    {
        public int TotalProjects { get; set; }
        public int ActiveProjects { get; set; }
        public int CompletedProjects { get; set; }
        public int TotalBoards { get; set; }
        public int TotalSprints { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }
        public int TotalNotifications { get; set; }
        public int TotalComments { get; set; }
        public int TotalAttachments { get; set; }

        public List<ProjectSummaryDTO> RecentProjects { get; set; } = new();
        public List<TaskSummaryDTO> RecentTasks { get; set; } = new();
        public List<TaskSummaryDTO> UpcomingDeadlines { get; set; } = new();
        public List<NotificationDTO> RecentNotifications { get; set; } = new();
        public SprintSummaryDTO? CurrentSprint { get; set; }
        public List<ProjectMemberSummaryDTO> TeamMembers { get; set; } = new();
    }
}
