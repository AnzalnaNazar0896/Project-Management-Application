namespace ProjectTracker.Models.Models.DTOs.Task
{
    public class TaskSummaryDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public int BoardId { get; set; }
        public string? BoardName { get; set; }
        public int? SprintId { get; set; }
        public string? SprintName { get; set; }
        public int ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public string? AssignedTo { get; set; }
        public int? AssignedEmployeeId { get; set; }
    }
}
