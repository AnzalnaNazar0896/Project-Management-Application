using ProjectTracker.Models.Models.DTOs.Attachment;
using ProjectTracker.Models.Models.DTOs.Board;
using ProjectTracker.Models.Models.DTOs.Notification;
using ProjectTracker.Models.Models.DTOs.Sprint;
using ProjectTracker.Models.Models.DTOs.Task;

namespace ProjectTracker.Models.Models.DTOs.Project
{
    public class ProjectDetailsDTO
    {
        public int Id { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
        public int Progress { get; set; }
        public bool IsCompleted { get; set; }

        public List<ProjectMemberSummaryDTO> Members { get; set; } = new();
        public List<BoardSummaryDTO> Boards { get; set; } = new();
        public List<SprintSummaryDTO> Sprints { get; set; } = new();
        public List<TaskSummaryDTO> Tasks { get; set; } = new();
        public List<AttachmentDTO> Attachments { get; set; } = new();
        public List<NotificationDTO> RecentNotifications { get; set; } = new();

        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }
        public int InProgressTasks { get; set; }

        public int TotalSprints { get; set; }
        public int ActiveSprints { get; set; }
        public int CompletedSprints { get; set; }
    }
}
