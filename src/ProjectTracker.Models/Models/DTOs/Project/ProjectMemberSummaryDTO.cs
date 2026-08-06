namespace ProjectTracker.Models.Models.DTOs.Project
{
    public class ProjectMemberSummaryDTO
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Availability { get; set; } = string.Empty;
        public int AssignedTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }
    }
}
