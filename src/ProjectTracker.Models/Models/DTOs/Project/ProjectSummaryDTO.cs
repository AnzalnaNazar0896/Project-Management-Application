namespace ProjectTracker.Models.Models.DTOs.Project
{
    public class ProjectSummaryDTO
    {
        public int Id { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string? Status { get; set; }
        public int Progress { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
