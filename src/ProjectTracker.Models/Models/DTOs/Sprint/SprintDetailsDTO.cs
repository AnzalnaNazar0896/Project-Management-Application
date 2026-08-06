using ProjectTracker.Models.Models.DTOs.Task;

namespace ProjectTracker.Models.Models.DTOs.Sprint
{
    public class SprintDetailsDTO
    {
        public int Id { get; set; }
        public string SprintName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public int Progress { get; set; }
        public List<TaskSummaryDTO> Tasks { get; set; } = new();
    }
}
