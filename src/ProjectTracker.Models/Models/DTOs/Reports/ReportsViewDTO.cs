namespace ProjectTracker.Models.Models.DTOs.Reports
{
    public class ReportsViewDTO
    {
        public List<ChartItemDTO> ProjectProgress { get; set; } = new();
        public List<ChartItemDTO> CompletedVsPending { get; set; } = new();
        public List<TaskSummaryItemDTO> OverdueTasks { get; set; } = new();
        public List<MemberAvailabilityDTO> MemberAvailability { get; set; } = new();
    }

    public class MemberAvailabilityDTO
    {
        public string MemberName { get; set; } = string.Empty;
        public string Availability { get; set; } = "Available";
    }
    public class ChartItemDTO
    {
        public string Label { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    public class TaskSummaryItemDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public string AssignedTo { get; set; } = string.Empty;
    }
}
